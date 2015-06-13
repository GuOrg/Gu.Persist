namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Settings.IO;

    public abstract class Repository : IRepository, IAsyncRepository, IAutoAsyncRepository, IAutoRepository, ICloner, IAutoSavingRepository, IDisposable
    {
        private readonly ConcurrentDictionary<FileInfo, WeakReference> _cache = new ConcurrentDictionary<FileInfo, WeakReference>(FileInfoComparer.Default);
        private bool _disposed;
        private readonly IBackuper _backuper = Settings.Backuper.None;

        /// <summary>
        /// Defaults to %AppDat%/Settings
        /// </summary>
        protected Repository()
            : this(Directories.ApplicationData.Subdirectory("Settings"))
        {
        }

        protected Repository(DirectoryInfo directory)
        {
            Ensure.NotNull(directory, "directory");
            directory.CreateIfNotExists();
            Setting = RepositorySetting.DefaultFor(directory);
            if (Setting.IsTrackingDirty)
            {
                Tracker = new DirtyTracker(this);
            }
            if (Exists<RepositorySetting>())
            {
                Setting = Read<RepositorySetting>();
            }
            else
            {
                Save(Setting);
            }
            _backuper = new Backuper(Setting.BackupSettings);
            Backuper.Repository = this;
        }

        protected Repository(RepositorySetting setting)
            : this(setting, new Backuper(setting.BackupSettings))
        {
        }

        public Repository(RepositorySetting setting, IBackuper backuper)
        {
            setting.Directory.CreateIfNotExists();
            Setting = setting;
            _backuper = backuper;
            Backuper.Repository = this;
            if (Setting.IsTrackingDirty)
            {
                Tracker = new DirtyTracker(this);
            }
        }

        public RepositorySetting Setting { get; private set; }

        public virtual IDirtyTracker Tracker { get; private set; }

        public virtual IBackuper Backuper
        {
            get { return _backuper; }
        }

        /// <summary>
        /// This gets the fileinfo used for reading & writing files of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual FileInfo GetFileInfo<T>()
        {
            return FileHelper.CreateFileInfo<T>(Setting);
        }

        public virtual bool Exists<T>()
        {
            var file = GetFileInfo<T>();
            return Exists<T>(file);
        }

        public virtual bool Exists<T>(string fileName)
        {
            var fileInfo = FileHelper.CreateFileInfo(fileName, Setting);
            return Exists<T>(fileInfo);
        }

        public virtual bool Exists<T>(FileInfo file)
        {
            file.Refresh();
            return file.Exists;
        }

        public virtual Task<T> ReadAsync<T>()
        {
            var file = GetFileInfo<T>();
            return ReadAsync<T>(file);
        }

        public virtual Task<T> ReadAsync<T>(string fileName)
        {
            var fileInfo = FileHelper.CreateFileInfo(fileName, Setting);
            return ReadAsync<T>(fileInfo);
        }

        public virtual async Task<T> ReadAsync<T>(FileInfo file)
        {
            VerifyDisposed();
            Ensure.NotNull(file, "file");
            WeakReference cached;
            if (_cache.TryGetValue(file, out cached))
            {
                return (T)cached.Target;
            }
            var value = await FileHelper.ReadAsync(file, FromStream<T>);
            _cache.TryAdd(file, new WeakReference(value));
            if (Setting.IsTrackingDirty)
            {
                Tracker.TrackOrUpdate(file, value);
            }
            return value;
        }

        public virtual T Read<T>()
        {
            var file = GetFileInfo<T>();
            return Read<T>(file);
        }

        public virtual T ReadOrCreate<T>(Func<T> creator)
        {
            var file = GetFileInfo<T>();
            return ReadOrCreate(file, creator);
        }

        /// <summary>
        /// Reads from file the first time. After that it returns returns cached value (singleton).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Optional if blank a file with the name of the class is read.</param>
        /// <returns></returns>
        public virtual T Read<T>(string fileName)
        {
            var fileInfo = FileHelper.CreateFileInfo(fileName, Setting);
            return Read<T>(fileInfo);
        }

        public virtual T ReadOrCreate<T>(string fileName, Func<T> creator)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            var file = FileHelper.CreateFileInfo(fileName, Setting);
            return ReadOrCreate(file, creator);
        }

        public virtual T Read<T>(FileInfo file)
        {
            VerifyDisposed();
            Ensure.NotNull(file, "file");
            WeakReference cached;
            if (_cache.TryGetValue(file, out cached))
            {
                return (T)cached.Target;
            }
            var value = FileHelper.Read(file, FromStream<T>);
            _cache.TryAdd(file, new WeakReference(value));
            if (Setting.IsTrackingDirty)
            {
                Tracker.TrackOrUpdate(file, value);
            }
            return value;
        }

        public virtual T ReadOrCreate<T>(FileInfo file, Func<T> creator)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNull(creator, "creator");
            T setting;
            if (Exists<T>())
            {
                setting = Read<T>();
            }
            else
            {
                setting = creator();
                Save(setting);
            }
            return setting;
        }

        /// <summary>
        /// Saves to a file named typeof(T).Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public virtual void Save<T>(T item)
        {
            var file = GetFileInfo<T>();
            Save(item, file);
        }

        public virtual void Save<T>(T item, string fileName)
        {
            var file = FileHelper.CreateFileInfo(fileName, Setting);
            Save(item, file);
        }

        public virtual void Save<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, "file");
            var tempFile = file.ChangeExtension(Setting.TempExtension);
            Save(item, file, tempFile);
        }

        public virtual void Save<T>(T item, FileInfo file, FileInfo tempFile)
        {
            VerifyDisposed();
            Ensure.NotNull(file, "file");
            Ensure.NotNull(tempFile, "tempFile");
            Cache(item, file);
            if (Setting.IsTrackingDirty)
            {
                Tracker.TrackOrUpdate(file, item);
            }

            Backuper.Backup(file);
            try
            {
                if (item == null)
                {
                    FileHelper.HardDelete(file);
                }
                else
                {
                    using (var stream = ToStream(item))
                    {
                        FileHelper.Save(tempFile, stream);
                    }
                    tempFile.MoveTo(file);
                    Backuper.PurgeBackups(file);
                }
            }
            catch (Exception)
            {
                Backuper.Restore(file);
                throw;
            }
        }

        public virtual Task SaveAsync<T>(T item)
        {
            var file = GetFileInfo<T>();
            return SaveAsync(item, file);
        }

        public virtual Task SaveAsync<T>(T item, string fileName)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            var fileInfo = FileHelper.CreateFileInfo(fileName, Setting);
            return SaveAsync(item, fileInfo);
        }

        public virtual Task SaveAsync<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, "file");
            var tempFile = file.ChangeExtension(Setting.TempExtension);
            return SaveAsync(item, file, tempFile);
        }

        public virtual async Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile)
        {
            VerifyDisposed();
            Ensure.NotNull(file, "file");
            Ensure.NotNull(tempFile, "tempFile");
            Cache(item, file);
            if (Setting.IsTrackingDirty)
            {
                Tracker.TrackOrUpdate(file, item);
            }
            Backuper.Backup(file);

            try
            {
                if (item == null)
                {
                    FileHelper.HardDelete(file);
                }
                else
                {
                    using (var stream = ToStream(item))
                    {
                        await FileHelper.SaveAsync(tempFile, stream)
                                        .ConfigureAwait(false);
                    }
                    tempFile.MoveTo(file);
                    Backuper.PurgeBackups(file);
                }
            }
            catch (Exception)
            {
                Backuper.Restore(file);
                throw;
            }
        }

        public virtual bool IsDirty<T>(T item)
        {
            return IsDirty(item, DefaultStructuralEqualityComparer<T>());
        }

        public virtual bool IsDirty<T>(T item, IEqualityComparer<T> comparer)
        {
            var file = GetFileInfo<T>();
            return IsDirty(item, file, comparer);
        }

        public virtual bool IsDirty<T>(T item, string fileName)
        {
            return IsDirty(item, fileName, DefaultStructuralEqualityComparer<T>());
        }

        public virtual bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            var fileInfo = FileHelper.CreateFileInfo(fileName, Setting);
            return IsDirty(item, fileInfo);
        }

        public virtual bool IsDirty<T>(T item, FileInfo file)
        {
            return IsDirty(item, file, DefaultStructuralEqualityComparer<T>());
        }

        public virtual bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            VerifyDisposed();
            if (!Setting.IsTrackingDirty)
            {
                throw new InvalidOperationException("Cannot check IsDirty if not Setting.IsTrackingDirty");
            }
            return Tracker.IsDirty(item, file, comparer);
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual T Clone<T>(T item)
        {
            VerifyDisposed();
            using (var stream = ToStream(item))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (disposing)
            {
                //FileHelper.Finished.WaitOne();
                // Intentional no-operation.
                // Using a transaction to wait for any current transaction has time to finish.
            }
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType()
                        .FullName);
            }
        }

        protected abstract T FromStream<T>(Stream stream);

        protected abstract Stream ToStream<T>(T item);

        protected abstract IEqualityComparer<T> DefaultStructuralEqualityComparer<T>();

        protected virtual void Cache<T>(T item, FileInfo file)
        {
            VerifyDisposed();
            WeakReference cached;
            if (_cache.TryGetValue(file, out cached))
            {
                if (!ReferenceEquals(item, cached.Target))
                {
                    throw new InvalidOperationException("Trying to save a different instance than the cached");
                }
            }
            else
            {
                _cache.TryAdd(file, new WeakReference(item));
            }
        }
    }
}
