#pragma warning disable 1573
namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for a repository.
    /// </summary>
    /// <typeparam name="TSetting">The type of setting.</typeparam>
    public abstract class Repository<TSetting> : IRepository,
                                                 IBlockingRepository,
                                                 IAsyncRepository,
                                                 IStreamRepository,
                                                 IRepositoryWithSettings
        where TSetting : IRepositorySettings
    {
        private readonly Serialize<TSetting> serialize;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with default settings.
        /// If directory contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        /// <param name="serialize">The <see cref="Serialize{TSetting}"/>.</param>
        protected Repository(Func<TSetting> settingsCreator, Serialize<TSetting> serialize)
        {
            if (settingsCreator is null)
            {
                throw new ArgumentNullException(nameof(settingsCreator));
            }

            this.serialize = serialize ?? throw new ArgumentNullException(nameof(serialize));
            this.Settings = settingsCreator();
            _ = Directory.CreateDirectory(this.Settings.Directory);
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker = new DirtyTracker(this);
            }

            // creating temporary backuper for TryRestore in ReadOrCreate
            this.Backuper = Backup.Backuper.Create(this.Settings.BackupSettings);
            var readSettings = this.ReadOrCreateCore(this.GetFileInfoCore<TSetting>(), () => this.Settings);
            if (!ReferenceEquals(readSettings, this.Settings))
            {
                this.Settings = readSettings;
                this.Backuper = Backup.Backuper.Create(readSettings.BackupSettings);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with default settings.
        /// If the directory contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backup settings.
        /// </param>
        /// <param name="serialize">The <see cref="Serialize{TSetting}"/>.</param>
        protected Repository(Func<TSetting> settingsCreator, IBackuper backuper, Serialize<TSetting> serialize)
        {
            if (settingsCreator is null)
            {
                throw new ArgumentNullException(nameof(settingsCreator));
            }

            this.serialize = serialize ?? throw new ArgumentNullException(nameof(serialize));
            this.Backuper = backuper ?? throw new ArgumentNullException(nameof(backuper));
            this.Settings = settingsCreator();
            _ = Directory.CreateDirectory(this.Settings.Directory);
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker = new DirtyTracker(this);
            }

            this.Settings = this.ReadOrCreateCore(this.GetFileInfoCore<TSetting>(), () => this.Settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with <paramref name="settings"/>.
        /// </summary>
        /// <param name="serialize">The <see cref="Serialize{TSetting}"/>.</param>
        /// <param name="settings">The <typeparamref name="TSetting"/>.</param>
        protected Repository(TSetting settings, Serialize<TSetting> serialize)
            : this(settings, Backup.Backuper.Create(settings.BackupSettings), serialize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with <paramref name="settings"/>.
        /// </summary>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backup settings.
        /// </param>
        /// <param name="serialize">The <see cref="Serialize{TSetting}"/>.</param>
        /// <param name="settings">The <typeparamref name="TSetting"/>.</param>
        protected Repository(TSetting settings, IBackuper backuper, Serialize<TSetting> serialize)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.serialize = serialize ?? throw new ArgumentNullException(nameof(serialize));
            new DirectoryInfo(settings.Directory).CreateIfNotExists();
            this.Settings = settings;
            this.Backuper = backuper;
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker = new DirtyTracker(this);
            }
        }

        /// <summary>
        /// Gets the <see cref="IRepository.Settings"/>.
        /// </summary>
        public TSetting Settings { get; }

        /// <inheritdoc/>
        IRepositorySettings IRepository.Settings => this.Settings;

        /// <inheritdoc/>
        IRepositorySettings IBlockingRepository.Settings => this.Settings;

        /// <inheritdoc/>
        IRepositorySettings IAsyncRepository.Settings => this.Settings;

        /// <inheritdoc/>
        IRepositorySettings IStreamRepository.Settings => this.Settings;

        /// <inheritdoc/>
        IRepositorySettings IRepositoryWithSettings.Settings => this.Settings;

        /// <inheritdoc/>
        public IDirtyTracker Tracker { get; }

        /// <inheritdoc/>
        public IBackuper Backuper { get; }

        /// <inheritdoc/>
        public virtual T Read<T>(FileInfo file, Migration migration = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return this.ReadCore<T>(file, migration);
        }

        /// <inheritdoc/>
        public virtual T Read<T>(string fileName, Migration migration = null)
        {
            var file = this.GetFileInfoCore(fileName);
            return this.ReadCore<T>(file, migration);
        }

        /// <inheritdoc/>
        public virtual T Read<T>(Migration migration = null)
        {
            return this.ReadCore<T>(this.GetFileInfo<T>(), migration);
        }

        /// <inheritdoc/>
        Stream IGenericStreamRepository.Read<T>()
        {
            var file = this.GetFileInfoCore<T>();
            return file.OpenRead();
        }

        /// <inheritdoc/>
        Stream IFileInfoStreamRepository.Read(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return file.OpenRead();
        }

        /// <inheritdoc/>
        Stream IFileNameStreamRepository.Read(string fileName)
        {
            var file = this.GetFileInfoCore(fileName);
            return file.OpenRead();
        }

        /// <inheritdoc/>
        public virtual async Task<T> ReadAsync<T>(FileInfo file, Migration migration = null)
        {
            // not checking exists, framework exception is more familiar.
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (migration is null)
            {
                using (var stream = await file.ReadAsync().ConfigureAwait(false))
                {
                    var value = this.serialize.FromStream<T>(stream, this.Settings);
                    if (this.Settings.IsTrackingDirty)
                    {
                        this.Tracker.Track(file.FullName, value);
                    }

                    return value;
                }
            }

            using (var stream = await file.ReadAsync().ConfigureAwait(false))
            {
                if (migration.TryUpdate(stream, out var updatedStream))
                {
                    using (updatedStream)
                    {
                        stream.Dispose();
                        var item = this.serialize.FromStream<T>(updatedStream, this.Settings);
                        //// Save so we get a backup etc.
                        this.Save(file, item);
                        return item;
                    }
                }

                stream.Position = 0;
                return this.serialize.FromStream<T>(stream, this.Settings);
            }
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadAsync<T>(string fileName, Migration migration = null)
        {
            var fileInfo = this.GetFileInfoCore(fileName);
            return this.ReadAsync<T>(fileInfo, migration);
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadAsync<T>(Migration migration = null)
        {
            var file = this.GetFileInfo<T>();
            return this.ReadAsync<T>(file, migration);
        }

        /// <inheritdoc/>
        Task<Stream> IFileInfoAsyncStreamRepository.ReadAsync(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return file.ReadAsync();
        }

        /// <inheritdoc/>
        Task<Stream> IFileNameAsyncStreamRepository.ReadAsync(string fileName)
        {
            var file = this.GetFileInfoCore(fileName);
            return file.ReadAsync();
        }

        /// <inheritdoc/>
        Task<Stream> IGenericAsyncStreamRepository.ReadAsync<T>()
        {
            var file = this.GetFileInfo<T>();
            return file.ReadAsync();
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(FileInfo file, Func<T> creator, Migration migration = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            return this.ReadOrCreateCore(file, creator);
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(string fileName, Func<T> creator, Migration migration = null)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            var file = this.GetFileInfoCore(fileName);
            return this.ReadOrCreateCore(file, creator);
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(Func<T> creator, Migration migration = null)
        {
            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            return this.ReadOrCreateCore(this.GetFileInfoCore<T>(), creator);
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadOrCreateAsync<T>(FileInfo file, Func<T> creator, Migration migration = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            return this.ReadOrCreateCoreAsync(file, creator, migration);
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadOrCreateAsync<T>(string fileName, Func<T> creator, Migration migration = null)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            var file = this.GetFileInfoCore(fileName);
            return this.ReadOrCreateCoreAsync(file, creator);
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadOrCreateAsync<T>(Func<T> creator, Migration migration = null)
        {
            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            return this.ReadOrCreateCoreAsync(this.GetFileInfoCore<T>(), creator);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(FileInfo file, FileInfo tempFile, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (tempFile is null)
            {
                throw new ArgumentNullException(nameof(tempFile));
            }

            this.EnsureCanSave(file, item);
            this.SaveCore(file, tempFile, item);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.EnsureCanSave(file, item);
            this.SaveCore(file, item);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(string fileName, T item)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var file = this.GetFileInfoCore(fileName);
            this.EnsureCanSave(file, item);
            this.SaveCore(file, item);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item)
        {
            var file = this.GetFileInfo<T>();
            this.EnsureCanSave(file, item);
            this.SaveCore(item);
        }

        /// <inheritdoc/>
        void IFileInfoStreamRepository.Save(FileInfo file, FileInfo tempFile, Stream stream)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.EnsureCanSave(file, stream);
            this.SaveStreamCore(file, tempFile, stream);
        }

        /// <inheritdoc/>
        void IFileInfoStreamRepository.Save(FileInfo file, Stream stream)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            this.SaveStreamCore(file, tempFile, stream);
        }

        /// <inheritdoc/>
        void IFileNameStreamRepository.Save(string fileName, Stream stream)
        {
            var file = this.GetFileInfoCore(fileName);
            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            this.SaveStreamCore(file, tempFile, stream);
        }

        /// <inheritdoc/>
        void IGenericStreamRepository.Save<T>(Stream stream)
        {
            var file = this.GetFileInfoCore<T>();
            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            this.SaveStreamCore(file, tempFile, stream);
        }

        /// <inheritdoc/>
        public virtual async Task SaveAsync<T>(FileInfo file, FileInfo tempFile, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (tempFile is null)
            {
                throw new ArgumentNullException(nameof(tempFile));
            }

            this.EnsureCanSave(file, item);
            this.CacheAndTrackCore(file, item);
            using (var stream = item != null
                ? this.serialize.ToStream(item, this.Settings)
                : null)
            {
                await this.SaveStreamCoreAsync(file, tempFile, stream)
                          .ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.EnsureCanSave(file, item);
            var tempFile = file.WithAppendedExtension(this.Settings.TempExtension);
            return this.SaveAsync(file, tempFile, item);
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(string fileName, T item)
        {
            var file = this.GetFileInfoCore(fileName);
            this.EnsureCanSave(file, item);
            return this.SaveAsync(file, item);
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(T item)
        {
            var file = this.GetFileInfo<T>();
            this.EnsureCanSave(file, item);
            return this.SaveAsync(file, item);
        }

        /// <inheritdoc/>
        Task IFileInfoAsyncStreamRepository.SaveAsync(FileInfo file, FileInfo tempFile, Stream stream)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.EnsureCanSave(file, stream);
            return this.SaveStreamCoreAsync(file, tempFile, stream);
        }

        /// <inheritdoc/>
        Task IFileInfoAsyncStreamRepository.SaveAsync(FileInfo file, Stream stream)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            return this.SaveStreamCoreAsync(file, tempFile, stream);
        }

        /// <inheritdoc/>
        Task IFileNameAsyncStreamRepository.SaveAsync(string fileName, Stream stream)
        {
            var file = this.GetFileInfoCore(fileName);
            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            return this.SaveStreamCoreAsync(file, tempFile, stream);
        }

        /// <inheritdoc/>
        Task IGenericAsyncStreamRepository.SaveAsync<T>(Stream stream)
        {
            var file = this.GetFileInfo<T>();
            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            return this.SaveStreamCoreAsync(file, tempFile, stream);
        }

        /// <inheritdoc/>
        public virtual FileInfo GetFileInfo(string fileName)
        {
            return this.GetFileInfoCore(fileName);
        }

        /// <inheritdoc/>
        public virtual FileInfo GetFileInfo<T>()
        {
            return this.GetFileInfoCore<T>();
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.Backuper.DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups(string fileName)
        {
            var file = this.GetFileInfoCore(fileName);
            this.DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups<T>()
        {
            var file = this.GetFileInfo<T>();
            this.DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual bool Exists(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return this.ExistsCore(file);
        }

        /// <inheritdoc/>
        public virtual bool Exists(string fileName)
        {
            var fileInfo = this.GetFileInfoCore(fileName);
            return this.Exists(fileInfo);
        }

        /// <inheritdoc/>
        public virtual bool Exists<T>()
        {
            return this.ExistsCore<T>();
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item)
        {
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            return this.IsDirty(item, this.serialize.DefaultStructuralEqualityComparer<T>(this.Settings));
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, IEqualityComparer<T> comparer)
        {
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            var file = this.GetFileInfo<T>();
            return this.IsDirty(file, item, comparer);
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(string fileName, T item)
        {
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            return this.IsDirty(fileName, item, this.serialize.DefaultStructuralEqualityComparer<T>(this.Settings));
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(string fileName, T item, IEqualityComparer<T> comparer)
        {
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            var fileInfo = this.GetFileInfoCore(fileName);
            return this.IsDirty(fileInfo, item, comparer);
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            return this.IsDirty(file, item, this.serialize.DefaultStructuralEqualityComparer<T>(this.Settings));
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(FileInfo file, T item, IEqualityComparer<T> comparer)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("Cannot check IsDirty if not Setting.IsTrackingDirty");
            }

            return this.Tracker.IsDirty(file.FullName, item, comparer);
        }

        /// <inheritdoc/>
        public bool CanRename<T>(string newName)
        {
            var fileInfo = this.GetFileInfo<T>();
            return this.CanRename(fileInfo, newName);
        }

        /// <inheritdoc/>
        public void Rename<T>(string newName, bool overWrite)
        {
            var fileInfo = this.GetFileInfo<T>();
            var newFile = this.GetFileInfoCore(newName);
            this.Rename(fileInfo, newFile, overWrite);
        }

        /// <inheritdoc/>
        public bool CanRename(string oldName, string newName)
        {
            var oldFile = this.GetFileInfoCore(oldName);
            var newFile = this.GetFileInfoCore(newName);
            return this.CanRename(oldFile, newFile);
        }

        /// <inheritdoc/>
        public void Rename(string oldName, string newName, bool overWrite)
        {
            var oldFile = this.GetFileInfoCore(oldName);
            var newFile = this.GetFileInfoCore(newName);
            this.Rename(oldFile, newFile, overWrite);
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo oldName, string newName)
        {
            if (oldName is null)
            {
                throw new ArgumentNullException(nameof(oldName));
            }

            var newFile = this.GetFileInfoCore(newName);
            return this.CanRename(oldName, newFile);
        }

        /// <inheritdoc/>
        public void Rename(FileInfo oldName, string newName, bool overWrite)
        {
            if (oldName is null)
            {
                throw new ArgumentNullException(nameof(oldName));
            }

            if (newName is null)
            {
                throw new ArgumentNullException(nameof(newName));
            }

            Ensure.Exists(oldName, nameof(oldName));
            var newFile = this.GetFileInfoCore(newName);
            this.Rename(oldName, newFile, overWrite);
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo oldName, FileInfo newName)
        {
            if (oldName is null)
            {
                throw new ArgumentNullException(nameof(oldName));
            }

            if (newName is null)
            {
                throw new ArgumentNullException(nameof(newName));
            }

            oldName.Refresh();
            if (!oldName.Exists)
            {
                return false;
            }

            newName.Refresh();
            if (newName.Exists)
            {
                return false;
            }

            if (this.Backuper != null)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newName.FullName);
                return this.Backuper.CanRename(oldName, fileNameWithoutExtension);
            }

            return true;
        }

        /// <inheritdoc/>
        public virtual void Rename(FileInfo oldName, FileInfo newName, bool overWrite)
        {
            if (oldName is null)
            {
                throw new ArgumentNullException(nameof(oldName));
            }

            if (newName is null)
            {
                throw new ArgumentNullException(nameof(newName));
            }

            var pairs = new List<RenamePair> { new RenamePair(oldName, newName) };
            var oldSoftDelete = oldName.SoftDeleteFile();
            if (oldSoftDelete?.Exists == true)
            {
                pairs.Add(new RenamePair(oldSoftDelete, newName.SoftDeleteFile()));
            }

            if (this.Backuper != null)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newName.Name);
                pairs.AddRange(this.Backuper.GetRenamePairs(oldName, fileNameWithoutExtension));
            }

            using (var transaction = new RenameTransaction(pairs))
            {
                transaction.Commit(overWrite);
            }
        }

        /// <inheritdoc/>
        public virtual T Clone<T>(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return this.serialize.Clone(item, this.Settings);
        }

        /// <inheritdoc/>
        public void ClearTrackerCache()
        {
            this.Tracker.ClearCache();
        }

        /// <inheritdoc/>
        public void RemoveFromDirtyTracker<T>(T item)
        {
            var tracker = this.Tracker;
            if (tracker == null)
            {
                return;
            }

            var file = this.GetFileInfo<T>();
            tracker.RemoveFromCache(file.FullName);
        }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> for that is used for the given filename.
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <returns>The <see cref="FileInfo"/>.</returns>
        protected FileInfo GetFileInfoCore(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var file = FileHelper.CreateFileInfo(fileName, this.Settings);
            return file;
        }

        /// <summary>
        /// Get the file that corresponds to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The <see cref="FileInfo"/>.</returns>
        protected FileInfo GetFileInfoCore<T>()
        {
            return FileHelper.CreateFileInfo<T>(this.Settings);
        }

        /// <summary>
        /// Read the file corresponding to <typeparamref name="T"/> and return it's contents deserialized to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to read and deserialize.</typeparam>
        /// <returns>The deserialized instance.</returns>
        protected T ReadCore<T>(Migration migration = null)
        {
            var file = this.GetFileInfoCore<T>();
            return this.ReadCore<T>(file, migration);
        }

        /// <summary>
        /// Read the file and return it's contents deserialized to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to read and deserialize.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <returns>The deserialized instance.</returns>
        protected virtual T ReadCore<T>(FileInfo file, Migration migration = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (migration is null)
            {
                var value = file.Read<T, TSetting>(this.Settings, this.serialize);
                if (this.Settings.IsTrackingDirty)
                {
                    this.Tracker.Track(file.FullName, value);
                }

                return value;
            }

            using (var stream = File.OpenRead(file.FullName))
            {
                if (migration.TryUpdate(stream, out var updatedStream))
                {
                    using (updatedStream)
                    {
                        stream.Dispose();
                        var item = this.serialize.FromStream<T>(updatedStream, this.Settings);
                        //// Save so we get a backup etc.
                        this.Save(file, item);
                        return item;
                    }
                }

                stream.Position = 0;
                return this.serialize.FromStream<T>(stream, this.Settings);
            }
        }

        /// <summary>
        /// Read the file and return it's contents deserialized to an instance of <typeparamref name="T"/> if it exists.
        /// If the file does not exist a new instance is created and saved, then this instance is returned.
        /// </summary>
        /// <typeparam name="T">The type to read and deserialize.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="creator">The <see cref="Func{T}"/>.</param>
        /// <returns>The deserialized instance.</returns>
        protected T ReadOrCreateCore<T>(FileInfo file, Func<T> creator, Migration migration = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            T item;
            if (file.Exists)
            {
                item = this.ReadCore<T>(file, migration);
            }
            else
            {
                item = creator();
                this.SaveCore(file, item);
            }

            return item;
        }

        /// <summary>
        /// Read the file and return it's contents deserialized to an instance of <typeparamref name="T"/> if it exists.
        /// If the file does not exist a new instance is created and saved, then this instance is returned.
        /// </summary>
        /// <typeparam name="T">The type to read and deserialize.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="creator">The <see cref="Func{T}"/>.</param>
        /// <returns>The deserialized instance.</returns>
        protected async Task<T> ReadOrCreateCoreAsync<T>(FileInfo file, Func<T> creator, Migration migration = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            T item;
            if (file.Exists)
            {
                item = await this.ReadAsync<T>(file, migration).ConfigureAwait(false);
            }
            else
            {
                item = creator();
                await this.SaveAsync(file, item).ConfigureAwait(false);
            }

            return item;
        }

        /// <summary>
        /// Save <paramref name="item"/> to a file corresponding to <typeparamref name="T"/>.
        /// Calls <see cref="SaveCore{T}(FileInfo,T)"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The <typeparamref name="T"/> to serialize and save.</param>
        protected void SaveCore<T>(T item)
        {
            var file = this.GetFileInfoCore<T>();
            this.SaveCore(file, item);
        }

        /// <summary>
        /// Calls <see cref="SaveCore{T}(FileInfo,FileInfo,T)"/>
        /// Uses file.WithNewExtension(this.Settings.TempExtension) as temp file.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The <typeparamref name="T"/> to serialize and save.</param>
        protected void SaveCore<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var tempFile = file.WithAppendedExtension(this.Settings.TempExtension);
            this.SaveCore(file, tempFile, item);
        }

        /// <summary>
        /// 1) All files in the transaction are locked.
        /// 2) If file exists it is renamed to file.delete
        /// 3) The contents of <paramref name="item"/> is saved to <paramref name="tempFile"/>
        /// 4.a 1) On success <paramref name="tempFile"/> is renamed to <paramref name="file"/>
        ///     2.a) If backup file.delete is renamed to backup name.
        ///     2.b) If no backup file.delete is deleted
        /// 4.b 1) file.delete is renamed back to file
        ///     2) <paramref name="tempFile"/> is deleted.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="tempFile">The temporary file to save to.</param>
        /// <param name="item">The <typeparamref name="T"/> to serialize and save.</param>
        protected void SaveCore<T>(FileInfo file, FileInfo tempFile, T item)
        {
            this.CacheAndTrackCore(file, item);
            using (var transaction = new SaveTransaction(file, tempFile, item, this.Backuper))
            {
                transaction.Commit(this.serialize, this.Settings);
            }
        }

        /// <summary>
        /// 1) All files in the transaction are locked.
        /// 2) If file exists it is renamed to file.delete
        /// 3) The contents of <paramref name="stream"/> is saved to <paramref name="tempFile"/>
        /// 4.a 1) On success <paramref name="tempFile"/> is renamed to <paramref name="file"/>
        ///     2.a) If backup file.delete is renamed to backup name.
        ///     2.b) If no backup file.delete is deleted
        /// 4.b 1) file.delete is renamed back to file
        ///     2) <paramref name="tempFile"/> is deleted.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="tempFile">The temporary file to save to.</param>
        /// <param name="stream">The <see cref="Stream"/>.</param>
        protected void SaveStreamCore(FileInfo file, FileInfo tempFile, Stream stream)
        {
            using (var transaction = new SaveTransaction(file, tempFile, stream, this.Backuper))
            {
                transaction.Commit(this.serialize, this.Settings);
            }
        }

        /// <summary>
        /// 1) All files in the transaction are locked.
        /// 2) If file exists it is renamed to file.delete
        /// 3) The contents of <paramref name="stream"/> is saved to <paramref name="tempFile"/>
        /// 4.a 1) On success <paramref name="tempFile"/> is renamed to <paramref name="file"/>
        ///     2.a) If backup file.delete is renamed to backup name.
        ///     2.b) If no backup file.delete is deleted
        /// 4.b 1) file.delete is renamed back to file
        ///     2) <paramref name="tempFile"/> is deleted.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="tempFile">The temporary file to save to.</param>
        /// <param name="stream">The <see cref="Stream"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task SaveStreamCoreAsync(FileInfo file, FileInfo tempFile, Stream stream)
        {
            using (var saveTransaction = new SaveTransaction(file, tempFile, stream, this.Backuper))
            {
                await saveTransaction.CommitAsync()
                                     .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Check if the file corresponding to <typeparamref name="T"/> exists.
        /// </summary>
        /// <typeparam name="T">The type to get corresponding file name for.</typeparam>
        /// <returns>True if a file exists.</returns>
        protected bool ExistsCore<T>()
        {
            var file = this.GetFileInfoCore<T>();
            return this.ExistsCore(file);
        }

        /// <summary>
        /// Check if <paramref name="file"/> exists.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <returns>True if the file exists.</returns>
        protected bool ExistsCore(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            file.Refresh();
            return file.Exists;
        }

        /// <summary>
        /// Handle caching and tracking for <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The <typeparamref name="T"/>.</param>
        protected virtual void CacheAndTrackCore<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker.Track(file.FullName, item);
            }
        }

        /// <summary>
        /// Throw exception if <paramref name="item"/> cannot be saved to <paramref name="file"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The <typeparamref name="T"/>.</param>
        protected abstract void EnsureCanSave<T>(FileInfo file, T item);
    }
}
