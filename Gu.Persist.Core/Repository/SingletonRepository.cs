﻿namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository that manages a singleton instance for files.
    /// Useful for settings.
    /// </summary>
    /// <typeparam name="TSetting">The type of settings.</typeparam>
    public abstract class SingletonRepository<TSetting> : Repository<TSetting>, ISingletonRepository
        where TSetting : IRepositorySettings
    {
        private readonly object gate = new();
        private readonly FileCache fileCache = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with default settings.
        /// If the directory contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        /// <param name="serialize">Serialization logic.</param>
        protected SingletonRepository(Func<TSetting> settingsCreator, Serialize<TSetting> serialize)
            : base(settingsCreator, serialize)
        {
            this.fileCache.Add(this.GetFileInfoCore<TSetting>().FullName, this.Settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with default settings.
        /// If the directory contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="serialize">Serialization logic.</param>
        protected SingletonRepository(Func<TSetting> settingsCreator, IBackuper backuper, Serialize<TSetting> serialize)
            : base(settingsCreator, backuper, serialize)
        {
            this.fileCache.Add(this.GetFileInfoCore<TSetting>().FullName, this.Settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">Setting controlling behavior.</param>
        /// <param name="serialize">Serialization logic.</param>
        protected SingletonRepository(TSetting settings, Serialize<TSetting> serialize)
            : this(settings, Backup.Backuper.Create(settings.BackupSettings), serialize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">Setting controlling behavior.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="serialize">Serialization logic.</param>
        protected SingletonRepository(TSetting settings, IBackuper backuper, Serialize<TSetting> serialize)
            : base(settings, backuper, serialize)
        {
        }

        /// <inheritdoc/>
        public override void Rename(FileInfo oldName, FileInfo newName, bool overWrite)
        {
            if (oldName is null)
            {
                throw new ArgumentNullException(nameof(oldName));
            }

            if (newName is null)
            {
                throw new ArgumentNullException(nameof(newName));
            }

            base.Rename(oldName, newName, overWrite);
            this.fileCache.ChangeKey(oldName.FullName, newName.FullName, overWrite);
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker?.Rename(oldName.FullName, newName.FullName, overWrite);
            }
        }

        /// <inheritdoc/>
        public override async Task<T> ReadAsync<T>(FileInfo file, Migration? migration = null)
        {
            // not checking exists, framework exception is more familiar.
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (this.fileCache.TryGetValue(file.FullName, out T? value))
            {
                return value ?? throw new InvalidOperationException("cached was null");
            }

            // can't await  inside the lock.
            // If there are many threads reading the same only the first is used
            // the other reads are wasted, can't think of anything better than this.
            value = await base.ReadAsync<T>(file, migration).ConfigureAwait(false);

            lock (this.gate)
            {
                if (this.fileCache.TryGetValue(file.FullName, out T? cached))
                {
                    return cached ?? throw new InvalidOperationException("cached was null");
                }

                this.fileCache.Add(file.FullName, value);
            }

            return value;
        }

        /// <inheritdoc/>
        public virtual void SaveAndClose<T>(T item)
        {
            var file = this.GetFileInfoCore<T>();
            this.EnsureCanSave(file, item);
            this.SaveAndCloseCore(file, item);
        }

        /// <inheritdoc/>
        public virtual void SaveAndClose<T>(string fileName, T item)
        {
            var file = this.GetFileInfoCore(fileName);
            this.EnsureCanSave(file, item);
            this.SaveAndCloseCore(file, item);
        }

        /// <inheritdoc/>
        public virtual void SaveAndClose<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.EnsureCanSave(file, item);
            this.SaveAndCloseCore(file, item);
        }

        /// <inheritdoc/>
        public void ClearCache()
        {
            this.fileCache.Clear();
        }

        /// <inheritdoc/>
        public void RemoveFromCache<T>(T item)
        {
            this.fileCache.TryRemove(item);
        }

        /// <inheritdoc/>
        protected override T ReadCore<T>(FileInfo file, Migration? migration = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (this.fileCache.TryGetValue(file.FullName, out T? value))
            {
                return value ?? throw new InvalidOperationException("cached was null");
            }

            lock (this.gate)
            {
                if (this.fileCache.TryGetValue(file.FullName, out value))
                {
                    return value ?? throw new InvalidOperationException("cached was null");
                }

                value = base.ReadCore<T>(file, migration);
                this.fileCache.Add(file.FullName, value);
                return value;
            }
        }

        /// <summary>
        /// Adds <paramref name="item"/> to the cache.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="CacheCore{T}(FileInfo,T)"/>.
        /// </remarks>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The item.</param>
        protected virtual void Cache<T>(FileInfo file, T item)
        {
            this.CacheCore(file, item);
        }

        /// <summary>
        /// Adds <paramref name="item"/> to the cache.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="CacheCore{T}(FileInfo,T)"/>.
        /// </remarks>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The item.</param>
        protected void CacheCore<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (this.fileCache.TryGetValue(file.FullName, out T? cached))
            {
                if (!ReferenceEquals(item, cached))
                {
                    throw new InvalidOperationException("Trying to save a different instance than the cached");
                }
            }
            else
            {
                this.fileCache.Add(file.FullName, item);
            }
        }

        /// <inheritdoc/>
        protected override void CacheAndTrackCore<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.Cache(file, item);
            base.CacheAndTrackCore(file, item);
        }

        /// <inheritdoc/>
        protected override void EnsureCanSave<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (item is null)
            {
                throw new ArgumentNullException($"{this.GetType().Name} cannot save null.");
            }

            if (this.fileCache.TryGetValue(file.FullName, out object? cached))
            {
                if (!ReferenceEquals(item, cached))
                {
                    throw new InvalidOperationException("Trying to save a different instance than the cached");
                }
            }
        }

        private void SaveAndCloseCore<T>(FileInfo file, T item)
        {
            this.SaveCore(file, item);
            this.RemoveFromCache(item);
            this.RemoveFromDirtyTracker(item);
        }
    }
}