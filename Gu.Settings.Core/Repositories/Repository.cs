namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Settings.Core.Backup;

    public abstract class Repository<TSetting> : IRepository, IGenericAsyncRepository, IAsyncFileNameRepository, ICloner, IAutoSavingRepository, IRepositoryWithSettings
        where TSetting : IRepositorySettings
    {
        private readonly object _gate = new object();
        private readonly FileCache _fileCache = new FileCache();
        [EditorBrowsable(EditorBrowsableState.Never)]
        private IBackuper _backuper;

        /// <summary>
        /// Defaults to %AppDat%/ExecutingAssembly.Name/Settings
        /// </summary>
        protected Repository()
        {
        }

        /// <summary>
        /// Creates a new <see cref="Repository"/> with default settings.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        protected Repository(DirectoryInfo directory, Func<TSetting> settingsCreator)
        {
            Ensure.NotNull(directory, nameof(directory));
            directory.CreateIfNotExists();
            Settings = settingsCreator();
            if (Settings.IsTrackingDirty)
            {
                Tracker = new DirtyTracker(this);
            }
            Backuper = Backup.Backuper.Create(Settings.BackupSettings); // creating temp for TryRestore in ReadOrCreate
            Settings = ReadOrCreateCore(() => (TSetting)Settings);
            Backuper = Backup.Backuper.Create(Settings.BackupSettings);
        }

        /// <summary>
        /// Creates a new <see cref="Repository"/> with <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings"></param>
        protected Repository(TSetting settings)
            : this(settings, Backup.Backuper.Create(settings.BackupSettings))
        {
        }

        /// <summary>
        /// Creates a new <see cref="Repository"/> with <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings"></param>
        protected Repository(TSetting settings, IBackuper backuper)
        {
            settings.DirectoryPath.CreateDirectoryInfo().CreateIfNotExists();
            Settings = settings;
            Backuper = backuper;
            if (Settings.IsTrackingDirty)
            {
                Tracker = new DirtyTracker(this);
            }
        }

        /// <inheritdoc/>
        public IRepositorySettings Settings { get; }
       
        /// <inheritdoc/>
        public IDirtyTracker Tracker { get; }

        /// <inheritdoc/>
        public IBackuper Backuper
        {
            get { return _backuper ?? NullBackuper.Default; }
            protected set
            {
                _backuper = value;
            }
        }

        /// <inheritdoc/>
        public virtual FileInfo GetFileInfo<T>()
        {
            return GetFileInfoCore<T>();
        }

        /// <inheritdoc/>
        public virtual FileInfo GetFileInfo(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            return GetFileInfoCore(fileName);
        }

        /// <inheritdoc/>
        protected FileInfo GetFileInfoCore(string fileName)
        {
            var file = FileHelper.CreateFileInfo(fileName, Settings);
            return file;
        }

        /// <inheritdoc/>
        protected FileInfo GetFileInfoCore<T>()
        {
            return FileHelper.CreateFileInfo<T>(Settings);
        }

        /// <inheritdoc/>
        public virtual void Delete<T>(bool deleteBackups)
        {
            var file = GetFileInfo<T>();
            Delete(file, deleteBackups);
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups<T>()
        {
            var file = GetFileInfo<T>();
            DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual void Delete(string fileName, bool deleteBackups)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = GetFileInfoCore(fileName);
            Delete(file, deleteBackups);
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = GetFileInfoCore(fileName);
            DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual void Delete(FileInfo file, bool deleteBackups)
        {
            Ensure.NotNull(file, nameof(file));
            file.Delete();
            file.DeleteSoftDeleteFileFor();
            if (deleteBackups)
            {
                DeleteBackups(file);
            }
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Backuper.DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual bool Exists<T>()
        {
            return ExistsCore<T>();
        }

        protected bool ExistsCore<T>()
        {
            var file = GetFileInfoCore<T>();
            return ExistsCore(file);
        }

        /// <inheritdoc/>
        public virtual bool Exists(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = GetFileInfoCore(fileName);
            return Exists(fileInfo);
        }

        /// <inheritdoc/>
        public virtual bool Exists(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return ExistsCore(file);
        }

        protected bool ExistsCore(FileInfo file)
        {
            file.Refresh();
            return file.Exists;
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadAsync<T>()
        {
            var file = GetFileInfo<T>();
            return ReadAsync<T>(file);
        }

        /// <inheritdoc/>
        public virtual Task<MemoryStream> ReadStreamAsync<T>()
        {
            var file = GetFileInfo<T>();
            return ReadStreamAsync(file);
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadAsync<T>(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = GetFileInfoCore(fileName);
            return ReadAsync<T>(fileInfo);
        }

        /// <inheritdoc/>
        public virtual Task<MemoryStream> ReadStreamAsync(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = GetFileInfoCore(fileName);
            return ReadStreamAsync(fileInfo);
        }

        /// <inheritdoc/>
        public virtual async Task<T> ReadAsync<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file)); // not checking exists, framework exception is more familiar.
            T value;
            if (Settings.IsCaching)
            {
                T cached;
                if (_fileCache.TryGetValue(file.FullName, out cached))
                {
                    return cached;
                }

                // can't await  inside the lock. 
                // If there are many threads reading the same only the first is used
                // the other reads are wasted, can't think of anything better than this.
                value = await FileHelper.ReadAsync<T>(file, FromStream<T>).ConfigureAwait(false);

                lock (_gate)
                {
                    if (_fileCache.TryGetValue(file.FullName, out cached))
                    {
                        return cached;
                    }
                    _fileCache.Add(file.FullName, value);
                }
            }
            else
            {
                value = await FileHelper.ReadAsync<T>(file, FromStream<T>).ConfigureAwait(false);
            }

            if (Settings.IsTrackingDirty)
            {
                Tracker.Track(file.FullName, value);
            }
            return value;
        }

        /// <inheritdoc/>
        public virtual Task<MemoryStream> ReadStreamAsync(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file)); // not checking exists, framework exception is more familiar.
            return file.ReadAsync();
        }

        /// <inheritdoc/>
        public virtual T Read<T>()
        {
            return ReadCore<T>();
        }

        /// <inheritdoc/>
        public virtual Stream ReadStream<T>()
        {
            var file = GetFileInfoCore<T>();
            return ReadStream(file);
        }

        protected T ReadCore<T>()
        {
            var file = GetFileInfoCore<T>();
            return ReadCore<T>(file);
        }

        /// <inheritdoc/>
        public virtual T Read<T>(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = GetFileInfoCore(fileName);
            return Read<T>(file);
        }

        /// <inheritdoc/>
        public virtual Stream ReadStream(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = GetFileInfoCore(fileName);
            return ReadStream(file);
        }

        /// <inheritdoc/>
        public virtual T Read<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return ReadCore<T>(file);
        }

        /// <inheritdoc/>
        public virtual Stream ReadStream(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return file.OpenRead();
        }

        protected T ReadCore<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            T value;
            if (Settings.IsCaching)
            {
                T cached;
                if (_fileCache.TryGetValue(file.FullName, out cached))
                {
                    return (T)cached;
                }

                lock (_gate)
                {
                    if (_fileCache.TryGetValue(file.FullName, out cached))
                    {
                        return (T)cached;
                    }
                    value = FileHelper.Read<T>(file, FromStream<T>);
                    _fileCache.Add(file.FullName, value);
                }
            }
            else
            {
                value = FileHelper.Read<T>(file, FromStream<T>);
            }

            if (Settings.IsTrackingDirty)
            {
                Tracker.Track(file.FullName, value);
            }

            return value;
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(Func<T> creator)
        {
            Ensure.NotNull(creator, nameof(creator));
            return ReadOrCreateCore(creator);
        }

        protected T ReadOrCreateCore<T>(Func<T> creator)
        {
            Ensure.NotNull(creator, nameof(creator));
            var file = GetFileInfoCore<T>();
            return ReadOrCreateCore(file, creator);
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(string fileName, Func<T> creator)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            Ensure.NotNull(creator, nameof(creator));
            var file = GetFileInfoCore(fileName);
            return ReadOrCreate(file, creator);
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(FileInfo file, Func<T> creator)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(creator, nameof(creator));
            return ReadOrCreateCore(file, creator);
        }

        protected T ReadOrCreateCore<T>(FileInfo file, Func<T> creator)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(creator, nameof(creator));
            T setting;
            if (ExistsCore<T>())
            {
                setting = ReadCore<T>();
            }
            else if (Backuper.TryRestore(file))
            {
                setting = ReadCore<T>();
            }
            else
            {
                setting = creator();
                SaveCore(setting);
            }
            return setting;
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item)
        {
            SaveCore(item);
        }

        /// <inheritdoc/>
        public virtual void SaveAndClose<T>(T item)
        {
            var file = GetFileInfoCore<T>();
            Save(item, file);
            RemoveFromCache(item);
            RemoveFromDirtyTracker(item);
        }

        protected void SaveCore<T>(T item)
        {
            var file = GetFileInfoCore<T>();
            SaveCore(item, file);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = GetFileInfoCore(fileName);
            Save(item, file);
        }

        /// <inheritdoc/>
        public virtual void SaveAndClose<T>(T item, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            Save(item, fileName);
            RemoveFromCache(item);
            RemoveFromDirtyTracker(item);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            SaveCore(item, file);
        }

        /// <inheritdoc/>
        public virtual void SaveAndClose<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            SaveCore(item, file);
            RemoveFromCache(item);
            RemoveFromDirtyTracker(item);
        }

        protected void SaveCore<T>(T item, FileInfo file)
        {
            var tempFile = file.WithNewExtension(Settings.TempExtension);
            SaveCore(item, file, tempFile);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item, FileInfo file, FileInfo tempFile)
        {
            Ensure.NotNull(file, nameof(file));
            SaveCore(item, file, tempFile);
        }

        /// <inheritdoc/>
        public virtual void SaveStream<T>(Stream stream)
        {
            var file = GetFileInfoCore<T>();
            SaveStream(stream, file);
        }

        /// <inheritdoc/>
        public virtual void SaveStream(Stream stream, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = GetFileInfoCore(fileName);
            SaveStream(stream, file);
        }

        /// <inheritdoc/>
        public virtual void SaveStream(Stream stream, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            var tempFile = file.WithNewExtension(Settings.TempExtension);
            SaveStreamCore(stream, file, tempFile);
        }

        /// <inheritdoc/>
        public virtual void SaveStream(Stream stream, FileInfo file, FileInfo tempFile)
        {
            Ensure.NotNull(file, nameof(file));
            SaveStreamCore(stream, file, tempFile);
        }

        protected void SaveCore<T>(T item, FileInfo file, FileInfo tempFile)
        {
            if (item == null)
            {
                SaveStreamCore(null, file, null);
                return;
            }
            CacheAndTrackCore(item, file);

            using (var stream = ToStream(item))
            {
                SaveStreamCore(stream, file, tempFile);
            }
        }

        protected void SaveStreamCore(Stream stream, FileInfo file, FileInfo tempFile)
        {
            if (stream == null)
            {
                FileHelper.HardDelete(file);
                return;
            }
            Backuper.BeforeSave(file);
            try
            {
                FileHelper.Save(tempFile, stream);
                tempFile.MoveTo(file);
                Backuper.AfterSuccessfulSave(file);
            }
            catch (Exception)
            {
                Backuper.TryRestore(file);
                throw;
            }
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(T item)
        {
            var file = GetFileInfo<T>();
            return SaveAsync(item, file);
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(T item, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = GetFileInfoCore(fileName);
            return SaveAsync(item, fileInfo);
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            var tempFile = file.WithNewExtension(Settings.TempExtension);
            return SaveAsync(item, file, tempFile);
        }

        /// <inheritdoc/>
        public virtual async Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(tempFile, nameof(tempFile));
            if (item == null)
            {
                FileHelper.HardDelete(file);
                return;
            }
            CacheAndTrackCore(item, file);
            using (var stream = ToStream(item))
            {
                await SaveStreamAsync(stream, file, tempFile).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual Task SaveStreamAsync<T>(Stream stream)
        {
            Ensure.NotNull(stream, nameof(stream));
            var file = GetFileInfo<T>();
            return SaveStreamAsync(stream, file);
        }

        /// <inheritdoc/>
        public virtual Task SaveStreamAsync(Stream stream, string fileName)
        {
            Ensure.NotNull(stream, nameof(stream));
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = GetFileInfoCore(fileName);
            return SaveStreamAsync(stream, file);
        }

        /// <inheritdoc/>
        public virtual Task SaveStreamAsync(Stream stream, FileInfo file)
        {
            Ensure.NotNull(stream, nameof(stream));
            Ensure.NotNull(file, nameof(file));
            var tempFile = file.WithNewExtension(Settings.TempExtension);
            return SaveStreamAsync(stream, file, tempFile);
        }

        /// <inheritdoc/>
        public virtual async Task SaveStreamAsync(Stream stream, FileInfo file, FileInfo tempFile)
        {
            Backuper.BeforeSave(file);
            try
            {
                await tempFile.SaveAsync(stream).ConfigureAwait(false);
                tempFile.MoveTo(file);
                Backuper.AfterSuccessfulSave(file);
            }
            catch (Exception)
            {
                Backuper.TryRestore(file);
                throw;
            }
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item)
        {
            return IsDirty(item, DefaultStructuralEqualityComparer<T>());
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, IEqualityComparer<T> comparer)
        {
            var file = GetFileInfo<T>();
            return IsDirty(item, file, comparer);
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            return IsDirty(item, fileName, DefaultStructuralEqualityComparer<T>());
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = GetFileInfoCore(fileName);
            return IsDirty(item, fileInfo, comparer);
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));

            return IsDirty(item, file, DefaultStructuralEqualityComparer<T>());
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            Ensure.NotNull(file, nameof(file));
            if (!Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("Cannot check IsDirty if not Setting.IsTrackingDirty");
            }
            return Tracker.IsDirty(item, file.FullName, comparer);
        }

        /// <inheritdoc/>
        public bool CanRename<T>(string newName)
        {
            Ensure.IsValidFileName(newName, nameof(newName));

            var fileInfo = GetFileInfo<T>();
            return CanRename(fileInfo, newName);
        }

        /// <inheritdoc/>
        public void Rename<T>(string newName, bool owerWrite)
        {
            Ensure.IsValidFileName(newName, nameof(newName));
            var fileInfo = GetFileInfo<T>();
            Rename(fileInfo, newName, owerWrite);
        }

        /// <inheritdoc/>
        public bool CanRename(string oldName, string newName)
        {
            Ensure.IsValidFileName(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var oldFile = GetFileInfoCore(oldName);
            var newFile = GetFileInfoCore(newName);
            return CanRename(oldFile, newFile);
        }

        /// <inheritdoc/>
        public void Rename(string oldName, string newName, bool owerWrite)
        {
            Ensure.IsValidFileName(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var oldFile = GetFileInfoCore(oldName);
            var newFile = GetFileInfoCore(newName);
            Rename(oldFile, newFile, owerWrite);
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo oldName, string newName)
        {
            Ensure.NotNull(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var newFile = GetFileInfoCore(newName);
            return CanRename(oldName, newFile);
        }

        /// <inheritdoc/>
        public void Rename(FileInfo oldName, string newName, bool owerWrite)
        {
            Ensure.Exists(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var newFile = GetFileInfoCore(newName);
            Rename(oldName, newFile, owerWrite);
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
            if (_fileCache.ContainsKey(newName.FullName))
            {
                return false;
            }
            if (Backuper != null)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newName.FullName);
                return Backuper.CanRename(oldName, fileNameWithoutExtension);
            }
            return true;
        }

        /// <inheritdoc/>
        public void Rename(FileInfo oldName, FileInfo newName, bool owerWrite)
        {
            Ensure.NotNull(oldName, nameof(oldName));
            Ensure.NotNull(newName, nameof(newName));
            oldName.Rename(newName, owerWrite);
            if (Backuper != null)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newName.Name);
                Backuper.Rename(oldName, fileNameWithoutExtension, owerWrite);
            }
            _fileCache.ChangeKey(oldName.FullName, newName.FullName, owerWrite);
            if (Settings.IsTrackingDirty && Tracker != null)
            {
                Tracker.Rename(oldName.FullName, newName.FullName, owerWrite);
            }
        }

        /// <inheritdoc/>
        public virtual T Clone<T>(T item)
        {
            Ensure.NotNull(item, nameof(item));
            return CloneCore(item);
        }

        public virtual T CloneCore<T>(T item)
        {
            Ensure.NotNull(item, nameof(item));
            using (var stream = ToStream(item))
            {
                return FromStream<T>(stream);
            }
        }

        /// <inheritdoc/>
        public void ClearCache()
        {
            _fileCache.Clear();
        }

        /// <inheritdoc/>
        public void RemoveFromCache<T>(T item)
        {
            _fileCache.TryRemove(item);
        }

        /// <inheritdoc/>
        public void ClearTrackerCache()
        {
            Tracker.ClearCache();
        }

        /// <inheritdoc/>
        public void RemoveFromDirtyTracker<T>(T item)
        {
            var tracker = Tracker;
            if (tracker == null)
            {
                return;
            }
            var fileInfo = GetFileInfo<T>();
            tracker.RemoveFromCache(fileInfo.FullName);
        }

        /// <summary>
        /// Deserialize from <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected abstract T FromStream<T>(Stream stream);

        /// <summary>
        /// Serialize from <paramref name="item"/> to a <see cref="Stream"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract Stream ToStream<T>(T item);

        /// <summary>
        /// Gets the comparer to use when checking <see cref="IDirty.IsDirty{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected abstract IEqualityComparer<T> DefaultStructuralEqualityComparer<T>();

        /// <summary>
        /// Adds <paramref name="item"/> to the cache.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="CacheCore{T}(T, FileInfo)"/>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        protected virtual void Cache<T>(T item, FileInfo file)
        {
            CacheCore(item, file);
        }

        protected void CacheCore<T>(T item, FileInfo file)
        {
            T cached;
            if (_fileCache.TryGetValue(file.FullName, out cached))
            {
                if (!ReferenceEquals(item, cached))
                {
                    throw new InvalidOperationException("Trying to save a different instance than the cached");
                }
            }
            else
            {
                _fileCache.Add(file.FullName, item);
            }
        }

        private void CacheAndTrackCore<T>(T item, FileInfo file)
        {
            if (Settings.IsCaching)
            {
                Cache(item, file);
            }

            if (Settings.IsTrackingDirty)
            {
                Tracker.Track(file.FullName, item);
            }
        }
    }
}
