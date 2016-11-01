#pragma warning disable 1573
namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for a repository
    /// </summary>
    public abstract class Repository<TSetting> :
        IRepository,
        IGenericAsyncRepository,
        IAsyncFileNameRepository,
        IAsyncFileInfoRepository,
        ICloner,
        IRepositoryWithSettings,
        IFileInfoStreamRepository,
        IGenericStreamRepository,
        IGenericAsyncStreamRepository,
        IFileNameStreamRepository,
        IFileNameAsyncStreamRepository
    where TSetting : IRepositorySettings
    {
        private readonly Serialize<TSetting> serialize;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with default settings.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        protected Repository(DirectoryInfo directory, Func<TSetting> settingsCreator, Serialize<TSetting> serialize)
        {
            Ensure.NotNull(directory, nameof(directory));
            Ensure.NotNull(settingsCreator, nameof(settingsCreator));
            Ensure.NotNull(serialize, nameof(serialize));
            this.serialize = serialize;
            directory.CreateIfNotExists();
            this.Settings = settingsCreator();
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker = new DirtyTracker(this);
            }

            this.Backuper = Backup.Backuper.Create(this.Settings.BackupSettings); // creating temp for TryRestore in ReadOrCreate
            this.Settings = this.ReadOrCreateCore(() => this.Settings);
            this.Backuper = Backup.Backuper.Create(this.Settings.BackupSettings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with default settings.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        protected Repository(DirectoryInfo directory, IBackuper backuper, Func<TSetting> settingsCreator, Serialize<TSetting> serialize)
        {
            Ensure.NotNull(directory, nameof(directory));
            Ensure.NotNull(backuper, nameof(backuper));
            Ensure.NotNull(settingsCreator, nameof(settingsCreator));
            Ensure.NotNull(serialize, nameof(serialize));
            this.serialize = serialize;
            directory.CreateIfNotExists();
            this.Settings = settingsCreator();
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker = new DirtyTracker(this);
            }

            this.Backuper = backuper;
            this.Settings = this.ReadOrCreateCore(() => this.Settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with <paramref name="settings"/>.
        /// </summary>
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
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        protected Repository(TSetting settings, IBackuper backuper, Serialize<TSetting> serialize)
        {
            Ensure.NotNull<object>(settings, nameof(settings));
            Ensure.NotNull(serialize, nameof(serialize));
            this.serialize = serialize;
            new DirectoryInfo(settings.Directory).CreateIfNotExists();
            this.Settings = settings;
            this.Backuper = backuper;
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker = new DirtyTracker(this);
            }
        }

        /// <summary>
        /// See <see cref="IRepository.Settings"/>
        /// </summary>
        public TSetting Settings { get; }

        IRepositorySettings IRepository.Settings => this.Settings;

        IRepositorySettings IRepositoryWithSettings.Settings => this.Settings;

        /// <inheritdoc/>
        public IDirtyTracker Tracker { get; }

        /// <inheritdoc/>
        public IBackuper Backuper { get; }

        /// <inheritdoc/>
        public virtual FileInfo GetFileInfo<T>()
        {
            return this.GetFileInfoCore<T>();
        }

        /// <inheritdoc/>
        public virtual FileInfo GetFileInfo(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            return this.GetFileInfoCore(fileName);
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups<T>()
        {
            var file = this.GetFileInfo<T>();
            this.DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            this.DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            this.Backuper.DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual bool Exists<T>()
        {
            return this.ExistsCore<T>();
        }

        /// <inheritdoc/>
        public virtual bool Exists(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = this.GetFileInfoCore(fileName);
            return this.Exists(fileInfo);
        }

        /// <inheritdoc/>
        public virtual bool Exists(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return this.ExistsCore(file);
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadAsync<T>()
        {
            var file = this.GetFileInfo<T>();
            return this.ReadAsync<T>(file);
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadAsync<T>(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = this.GetFileInfoCore(fileName);
            return this.ReadAsync<T>(fileInfo);
        }

        /// <inheritdoc/>
        public virtual async Task<T> ReadAsync<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file)); // not checking exists, framework exception is more familiar.
            var value = await FileHelper.ReadAsync(file, this.serialize.FromStream<T>).ConfigureAwait(false);
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker.Track(file.FullName, value);
            }

            return value;
        }

        /// <inheritdoc/>
        public virtual T Read<T>()
        {
            return this.ReadCore<T>();
        }

        /// <inheritdoc/>
        Task<Stream> IGenericAsyncStreamRepository.ReadAsync<T>()
        {
            var file = this.GetFileInfo<T>();
            return file.ReadAsync();
        }

        /// <inheritdoc/>
        Task<Stream> IFileNameAsyncStreamRepository.ReadAsync(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            return file.ReadAsync();
        }

        /// <inheritdoc/>
        Task<Stream> IFileInfoStreamRepository.ReadAsync(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return file.ReadAsync();
        }

        /// <inheritdoc/>
        public virtual T Read<T>(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            return this.Read<T>(file);
        }

        /// <inheritdoc/>
        public virtual T Read<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return this.ReadCore<T>(file);
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
            Ensure.NotNull(file, nameof(file));
            return file.OpenRead();
        }

        /// <inheritdoc/>
        Stream IFileNameStreamRepository.Read(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            return file.OpenRead();
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(Func<T> creator)
        {
            Ensure.NotNull(creator, nameof(creator));
            return this.ReadOrCreateCore(creator);
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(string fileName, Func<T> creator)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            Ensure.NotNull(creator, nameof(creator));
            var file = this.GetFileInfoCore(fileName);
            return this.ReadOrCreate(file, creator);
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(FileInfo file, Func<T> creator)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(creator, nameof(creator));
            return this.ReadOrCreateCore(file, creator);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item)
        {
            var file = this.GetFileInfo<T>();
            this.EnsureCanSave(file, item);
            this.SaveCore(item);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(string fileName, T item)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            this.EnsureCanSave(file, item);
            this.SaveCore(file, item);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(FileInfo file, T item)
        {
            Ensure.NotNull(file, nameof(file));
            this.EnsureCanSave(file, item);
            this.SaveCore(file, item);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(FileInfo file, FileInfo tempFile, T item)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(tempFile, nameof(tempFile));
            this.EnsureCanSave(file, item);
            this.SaveCore(file, tempFile, item);
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(T item)
        {
            var file = this.GetFileInfo<T>();
            this.EnsureCanSave(file, item);
            return this.SaveAsync(file, item);
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(string fileName, T item)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            this.EnsureCanSave(file, item);
            return this.SaveAsync(file, item);
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(FileInfo file, T item)
        {
            Ensure.NotNull(file, nameof(file));
            this.EnsureCanSave(file, item);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            return this.SaveAsync(file, tempFile, item);
        }

        /// <inheritdoc/>
        public virtual async Task SaveAsync<T>(FileInfo file, FileInfo tempFile, T item)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(tempFile, nameof(tempFile));
            this.EnsureCanSave(file, item);
            this.CacheAndTrackCore(file, item);
            using (var stream = item != null ? this.serialize.ToStream(item) : null)
            {
                await this.SaveStreamCoreAsync(file, tempFile, stream).ConfigureAwait(false);
            }
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
        void IFileNameStreamRepository.Save(string fileName, Stream stream)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            this.SaveStreamCore(file, tempFile, stream);
        }

        /// <inheritdoc/>
        void IFileInfoStreamRepository.Save(FileInfo file, Stream stream)
        {
            Ensure.NotNull(file, nameof(file));
            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            this.SaveStreamCore(file, tempFile, stream);
        }

        /// <inheritdoc/>
        void IFileInfoStreamRepository.Save(FileInfo file, FileInfo tempFile, Stream stream)
        {
            Ensure.NotNull(file, nameof(file));
            this.EnsureCanSave(file, stream);
            this.SaveStreamCore(file, tempFile, stream);
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
        Task IFileNameAsyncStreamRepository.SaveAsync(string fileName, Stream stream)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            return this.SaveStreamCoreAsync(file, tempFile, stream);
        }

        /// <inheritdoc/>
        Task IFileInfoStreamRepository.SaveAsync(FileInfo file, Stream stream)
        {
            Ensure.NotNull(file, nameof(file));
            this.EnsureCanSave(file, stream);
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            return this.SaveStreamCoreAsync(file, tempFile, stream);
        }

        /// <inheritdoc/>
        Task IFileInfoStreamRepository.SaveAsync(FileInfo file, FileInfo tempFile, Stream stream)
        {
            Ensure.NotNull(file, nameof(file));
            this.EnsureCanSave(file, stream);
            return this.SaveStreamCoreAsync(file, tempFile, stream);
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item)
        {
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            return this.IsDirty(item, this.serialize.DefaultStructuralEqualityComparer<T>());
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

            Ensure.IsValidFileName(fileName, nameof(fileName));
            return this.IsDirty(fileName, item, this.serialize.DefaultStructuralEqualityComparer<T>());
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(string fileName, T item, IEqualityComparer<T> comparer)
        {
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = this.GetFileInfoCore(fileName);
            return this.IsDirty(fileInfo, item, comparer);
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(FileInfo file, T item)
        {
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            Ensure.NotNull(file, nameof(file));

            return this.IsDirty(file, item, this.serialize.DefaultStructuralEqualityComparer<T>());
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(FileInfo file, T item, IEqualityComparer<T> comparer)
        {
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("This repository is not tracking dirty.");
            }

            Ensure.NotNull(file, nameof(file));
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("Cannot check IsDirty if not Setting.IsTrackingDirty");
            }

            return this.Tracker.IsDirty(file.FullName, item, comparer);
        }

        /// <inheritdoc/>
        public bool CanRename<T>(string newName)
        {
            Ensure.IsValidFileName(newName, nameof(newName));
            var fileInfo = this.GetFileInfo<T>();
            return this.CanRename(fileInfo, newName);
        }

        /// <inheritdoc/>
        public void Rename<T>(string newName, bool owerWrite)
        {
            Ensure.IsValidFileName(newName, nameof(newName));
            var fileInfo = this.GetFileInfo<T>();
            this.Rename(fileInfo, newName, owerWrite);
        }

        /// <inheritdoc/>
        public bool CanRename(string oldName, string newName)
        {
            Ensure.IsValidFileName(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var oldFile = this.GetFileInfoCore(oldName);
            var newFile = this.GetFileInfoCore(newName);
            return this.CanRename(oldFile, newFile);
        }

        /// <inheritdoc/>
        public void Rename(string oldName, string newName, bool overWrite)
        {
            Ensure.IsValidFileName(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var oldFile = this.GetFileInfoCore(oldName);
            var newFile = this.GetFileInfoCore(newName);
            this.Rename(oldFile, newFile, overWrite);
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo oldName, string newName)
        {
            Ensure.NotNull(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var newFile = this.GetFileInfoCore(newName);
            return this.CanRename(oldName, newFile);
        }

        /// <inheritdoc/>
        public void Rename(FileInfo oldName, string newName, bool overWrite)
        {
            Ensure.Exists(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var newFile = this.GetFileInfoCore(newName);
            this.Rename(oldName, newFile, overWrite);
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo oldName, FileInfo newName)
        {
            Ensure.NotNull(oldName, nameof(oldName));
            Ensure.NotNull(newName, nameof(newName));
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
            Ensure.NotNull(oldName, nameof(oldName));
            Ensure.NotNull(newName, nameof(newName));
            var pairs = new List<RenamePair> { new RenamePair(oldName, newName) };
            var oldSoftDelete = oldName.GetSoftDeleteFileFor();
            if (oldSoftDelete?.Exists == true)
            {
                pairs.Add(new RenamePair(oldSoftDelete, newName.GetSoftDeleteFileFor()));
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

            return this.serialize.Clone(item);
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
        /// Gets the fileinfo for that is used for the given filename
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        protected FileInfo GetFileInfoCore(string fileName)
        {
            var file = FileHelper.CreateFileInfo(fileName, this.Settings);
            return file;
        }

        /// <summary>
        /// Get the file that corresponds to <typeparamref name="T"/>
        /// </summary>
        protected FileInfo GetFileInfoCore<T>()
        {
            return FileHelper.CreateFileInfo<T>(this.Settings);
        }

        /// <summary>
        /// Read the file corresponding to <typeparamref name="T"/> and return it's contents deserialized to an instance of <typeparamref name="T"/>
        /// </summary>
        protected T ReadCore<T>()
        {
            var file = this.GetFileInfoCore<T>();
            return this.ReadCore<T>(file);
        }

        /// <summary>
        /// Read the file and return it's contents deserialized to an instance of <typeparamref name="T"/>
        /// </summary>
        protected virtual T ReadCore<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            var value = FileHelper.Read(file, this.serialize.FromStream<T>);
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker.Track(file.FullName, value);
            }

            return value;
        }

        /// <summary>
        /// Read the file corresponding to <typeparamref name="T"/> return it's contents deserialized to an instance of <typeparamref name="T"/> if it exists.
        /// If the file does not exist a new instance is created and saved, then this instance is returned.
        /// </summary>
        protected T ReadOrCreateCore<T>(Func<T> creator)
        {
            Ensure.NotNull(creator, nameof(creator));
            var file = this.GetFileInfoCore<T>();
            return this.ReadOrCreateCore(file, creator);
        }

        /// <summary>
        /// Read the file and return it's contents deserialized to an instance of <typeparamref name="T"/> if it exists.
        /// If the file does not exist a new instance is created and saved, then this instance is returned.
        /// </summary>
        protected T ReadOrCreateCore<T>(FileInfo file, Func<T> creator)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(creator, nameof(creator));
            T item;
            if (this.ExistsCore<T>())
            {
                item = this.ReadCore<T>();
            }
            else if (this.Backuper.TryRestore(file))
            {
                item = this.ReadCore<T>();
            }
            else
            {
                item = creator();
                this.SaveCore(item);
            }

            return item;
        }

        /// <summary>
        /// Save <paramref name="item"/> to a file corresponding to <typeparamref name="T"/>
        /// </summary>
        protected void SaveCore<T>(T item)
        {
            var file = this.GetFileInfoCore<T>();
            this.SaveCore(file, item);
        }

        /// <summary>
        /// Calls <see cref="SaveCore{T}(FileInfo,FileInfo,T)"/>
        /// Uses file.WithNewExtension(this.Settings.TempExtension) as temp file.
        /// </summary>
        protected void SaveCore<T>(FileInfo file, T item)
        {
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            this.SaveCore(file, tempFile, item);
        }

        /// <summary>
        /// Caches and tracks if needed.
        /// Then serializes and saves.
        /// </summary>
        protected void SaveCore<T>(FileInfo file, FileInfo tempFile, T item)
        {
            this.CacheAndTrackCore(file, item);
            using (var transaction = new SaveTransaction(file, tempFile, item, this.Backuper))
            {
                transaction.Commit(this.serialize, this.Settings);
            }
        }

        /// <summary>
        /// 1) If file exists it is renamed to file.delete
        /// 2) The contents of <paramref name="stream"/> is saved to tempFile
        /// 3.a 1) On success tempfile is renamed to <paramref name="file"/>
        ///     2.a) If backup file.delete is renamed to backup name.
        ///     2.b) If no backup file.delete is deleted
        /// 3.b 1) file.delete is renamed back to file
        ///     2) tempfile is deleted
        /// </summary>
        protected void SaveStreamCore(FileInfo file, FileInfo tempFile, Stream stream)
        {
            using (var transaction = new SaveTransaction(file, tempFile, stream, this.Backuper))
            {
                transaction.Commit(this.serialize, this.Settings);
            }
        }

        protected async Task SaveStreamCoreAsync(FileInfo file, FileInfo tempFile, Stream stream)
        {
            using (var saveTransaction = new SaveTransaction(file, tempFile, stream, this.Backuper))
            {
                await saveTransaction.CommitAsync()
                                     .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Check if the file corresponding to <typeparamref name="T"/> exists
        /// </summary>
        protected bool ExistsCore<T>()
        {
            var file = this.GetFileInfoCore<T>();
            return this.ExistsCore(file);
        }

        /// <summary>
        /// Check if <paramref name="file"/> exists
        /// </summary>
        protected bool ExistsCore(FileInfo file)
        {
            file.Refresh();
            return file.Exists;
        }

        protected virtual void CacheAndTrackCore<T>(FileInfo file, T item)
        {
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker.Track(file.FullName, item);
            }
        }

        protected abstract void EnsureCanSave<T>(FileInfo file, T item);
    }
}
