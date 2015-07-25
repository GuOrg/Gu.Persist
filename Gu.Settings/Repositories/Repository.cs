namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gu.Settings.Backup;
    using Internals;

    public abstract class Repository : IRepository, IGenericAsyncRepository, IAsyncFileNameRepository, ICloner, IAutoSavingRepository, IRepositoryWithSettings, IDisposable
    {
        private readonly ConcurrentDictionary<string, WeakReference> _cache = new ConcurrentDictionary<string, WeakReference>(StringComparer.OrdinalIgnoreCase);
        private bool _disposed;
        [EditorBrowsable(EditorBrowsableState.Never)]
        private IBackuper _backuper;

        /// <summary>
        /// Defaults to %AppDat%/ExecutingAssembly.Name/Settings
        /// </summary>
        protected Repository()
        {
        }

        protected Repository(DirectoryInfo directory)
        {
            Ensure.NotNull(directory, "directory");
            directory.CreateIfNotExists();
            Settings = RepositorySettings.DefaultFor(directory);
            if (Settings.IsTrackingDirty)
            {
                Tracker = new DirtyTracker(this);
            }
            Backuper = Backup.Backuper.Create(Settings.BackupSettings); // creating temp for TryRestore in ReadOrCreate
            Settings = ReadOrCreateCore(() => RepositorySettings.DefaultFor(directory));
            Backuper = Backup.Backuper.Create(Settings.BackupSettings);
        }

        protected Repository(IRepositorySettings settings)
            : this(settings, Backup.Backuper.Create(settings.BackupSettings))
        {
        }

        protected Repository(IRepositorySettings settings, IBackuper backuper)
        {
            settings.Directory.CreateIfNotExists();
            Settings = settings;
            Backuper = backuper;
            if (Settings.IsTrackingDirty)
            {
                Tracker = new DirtyTracker(this);
            }
        }

        public IRepositorySettings Settings { get; protected set; }

        public IDirtyTracker Tracker { get; protected set; }

        public IBackuper Backuper
        {
            get { return _backuper ?? NullBackuper.Default; }
            protected set
            {
                _backuper = value;
            }
        }

        /// <summary>
        /// This gets the fileinfo used for reading & writing files of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual FileInfo GetFileInfo<T>()
        {
            VerifyDisposed();
            return GetFileInfoCore<T>();
        }

        public virtual FileInfo GetFileInfo(string fileName)
        {
            Ensure.IsValidFileName(fileName, "fileName");
            VerifyDisposed();
            var file = FileHelper.CreateFileInfo(fileName, Settings);
            return file;
        }

        protected FileInfo GetFileInfoCore<T>()
        {
            return FileHelper.CreateFileInfo<T>(Settings);
        }

        public virtual void Delete<T>(bool deleteBackups)
        {
            VerifyDisposed();
            var file = GetFileInfo<T>();
            Delete(file, deleteBackups);
        }

        public virtual void Delete(string fileName, bool deleteBackups)
        {
            Ensure.IsValidFileName(fileName, "fileName");
            VerifyDisposed();
            var file = GetFileInfo(fileName);
            Delete(file, deleteBackups);
        }

        public virtual void Delete(FileInfo file, bool deleteBackups)
        {
            Ensure.NotNull(file, "file");
            VerifyDisposed();
            file.Delete();
            if (deleteBackups)
            {
                Backuper.DeleteBackups(file);
            }
        }

        public virtual bool Exists<T>()
        {
            VerifyDisposed();
            return ExistsCore<T>();
        }

        protected bool ExistsCore<T>()
        {
            var file = GetFileInfoCore<T>();
            return ExistsCore(file);
        }

        public virtual bool Exists(string fileName)
        {
            Ensure.IsValidFileName(fileName, "fileName");
            VerifyDisposed();
            var fileInfo = GetFileInfo(fileName);
            return Exists(fileInfo);
        }

        public virtual bool Exists(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            VerifyDisposed();
            return ExistsCore(file);
        }

        protected bool ExistsCore(FileInfo file)
        {
            file.Refresh();
            return file.Exists;
        }

        public virtual Task<T> ReadAsync<T>()
        {
            VerifyDisposed();
            var file = GetFileInfo<T>();
            return ReadAsync<T>(file);
        }

        public virtual Task<T> ReadAsync<T>(string fileName)
        {
            Ensure.IsValidFileName(fileName, "fileName");
            VerifyDisposed();
            var fileInfo = GetFileInfo(fileName);
            return ReadAsync<T>(fileInfo);
        }

        public virtual async Task<T> ReadAsync<T>(FileInfo file)
        {
            Ensure.NotNull(file, "file"); // not checking exists, framework exception is more familiar.
            VerifyDisposed();
            if (Settings.IsCaching)
            {
                WeakReference cached;
                if (_cache.TryGetValue(file.FullName, out cached))
                {
                    return (T)cached.Target;
                }
            }

            var value = await FileHelper.ReadAsync<T>(file, FromStream<T>);
            if (Settings.IsCaching)
            {
                _cache.TryAdd(file.FullName, new WeakReference(value));
            }
            if (Settings.IsTrackingDirty)
            {
                Tracker.TrackOrUpdate(file, value);
            }
            return value;
        }

        public virtual T Read<T>()
        {
            VerifyDisposed();
            return ReadCore<T>();
        }

        protected T ReadCore<T>()
        {
            var file = GetFileInfoCore<T>();
            return ReadCore<T>(file);
        }

        /// <summary>
        /// Reads from file the first time. After that it returns returns cached value (singleton).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Optional if blank a file with the name of the class is read.</param>
        /// <returns></returns>
        public virtual T Read<T>(string fileName)
        {
            Ensure.IsValidFileName(fileName, "fileName");
            VerifyDisposed();
            var fileInfo = GetFileInfo(fileName);
            return Read<T>(fileInfo);
        }

        public virtual T Read<T>(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            VerifyDisposed();
            return ReadCore<T>(file);
        }

        protected T ReadCore<T>(FileInfo file)
        {
            VerifyDisposed();
            Ensure.NotNull(file, "file");
            if (Settings.IsCaching)
            {
                WeakReference cached;
                if (_cache.TryGetValue(file.FullName, out cached))
                {
                    return (T)cached.Target;
                }
            }

            var value = FileHelper.Read<T>(file, FromStream<T>);
            if (Settings.IsCaching)
            {
                _cache.TryAdd(file.FullName, new WeakReference(value));
            }
            if (Settings.IsTrackingDirty)
            {
                Tracker.TrackOrUpdate(file, value);
            }
            return value;
        }

        public virtual T ReadOrCreate<T>(Func<T> creator)
        {
            Ensure.NotNull(creator, "creator");
            VerifyDisposed();
            return ReadOrCreateCore(creator);
        }

        protected T ReadOrCreateCore<T>(Func<T> creator)
        {
            Ensure.NotNull(creator, "creator");
            var file = GetFileInfoCore<T>();
            return ReadOrCreateCore(file, creator);
        }

        public virtual T ReadOrCreate<T>(string fileName, Func<T> creator)
        {
            Ensure.IsValidFileName(fileName, "fileName");
            Ensure.NotNull(creator, "creator");
            VerifyDisposed();
            var file = GetFileInfo(fileName);
            return ReadOrCreate(file, creator);
        }

        public virtual T ReadOrCreate<T>(FileInfo file, Func<T> creator)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNull(creator, "creator");
            VerifyDisposed();
            return ReadOrCreateCore(file, creator);
        }

        protected T ReadOrCreateCore<T>(FileInfo file, Func<T> creator)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNull(creator, "creator");
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

        /// <summary>
        /// Saves to a file named typeof(T).Name
        /// Note: T must be the same exact type you read.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public virtual void Save<T>(T item)
        {
            Ensure.NotNull(item, "item");
            VerifyDisposed();
            SaveCore(item);
        }

        protected void SaveCore<T>(T item)
        {
            var file = GetFileInfoCore<T>();
            SaveCore(item, file);
        }

        public virtual void Save<T>(T item, string fileName)
        {
            Ensure.NotNull(item, "item");
            Ensure.IsValidFileName(fileName, "fileName");
            VerifyDisposed();
            var file = GetFileInfo(fileName);
            Save(item, file);
        }

        /// <summary>
        /// Saves the file. Then removes it from cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fileName"></param>
        public virtual void SaveAndClose<T>(T item, string fileName)
        {
            Ensure.NotNull(item, "item");
            Ensure.IsValidFileName(fileName, "fileName");
            VerifyDisposed();
            Save(item, fileName);
            RemoveFromCache(item);
            RemoveFromDirtyTracker(item);
        }

        public virtual void Save<T>(T item, FileInfo file)
        {
            Ensure.NotNull(item, "item");
            Ensure.NotNull(file, "file");
            VerifyDisposed();
            SaveCore(item, file);
        }

        /// <summary>
        /// Saves the file. Then removes it from cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        public virtual void SaveAndClose<T>(T item, FileInfo file)
        {
            Ensure.NotNull(item, "item");
            Ensure.NotNull(file, "file");
            VerifyDisposed();
            SaveCore(item, file);
            RemoveFromCache(item);
            RemoveFromDirtyTracker(item);
        }

        protected void SaveCore<T>(T item, FileInfo file)
        {
            Ensure.NotNull(item, "item");
            Ensure.NotNull(file, "file");
            VerifyDisposed();
            var tempFile = file.WithNewExtension(Settings.TempExtension);
            SaveCore(item, file, tempFile);
        }

        public virtual void Save<T>(T item, FileInfo file, FileInfo tempFile)
        {
            Ensure.NotNull(item, "item");
            Ensure.NotNull(file, "file");
            VerifyDisposed();
            SaveCore(item, file, tempFile);
        }

        protected void SaveCore<T>(T item, FileInfo file, FileInfo tempFile)
        {
            var createdBackup = PrepareForSaveCore(item, file);
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
                Backuper.TryRestore(file);
                throw;
            }
        }

        public virtual Task SaveAsync<T>(T item)
        {
            Ensure.NotNull(item, "item");
            var file = GetFileInfo<T>();
            return SaveAsync(item, file);
        }

        public virtual Task SaveAsync<T>(T item, string fileName)
        {
            Ensure.NotNull(item, "item");
            Ensure.IsValidFileName(fileName, "fileName");
            VerifyDisposed();
            var fileInfo = GetFileInfo(fileName);
            return SaveAsync(item, fileInfo);
        }

        public virtual Task SaveAsync<T>(T item, FileInfo file)
        {
            Ensure.NotNull(item, "item");
            Ensure.NotNull(file, "file");
            VerifyDisposed();
            var tempFile = file.WithNewExtension(Settings.TempExtension);
            return SaveAsync(item, file, tempFile);
        }

        public virtual async Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNull(tempFile, "tempFile");
            VerifyDisposed();

            var createdBackup = PrepareForSaveCore(item, file);

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
                Backuper.TryRestore(file);
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
            Ensure.IsValidFileName(fileName, "fileName");
            VerifyDisposed();
            return IsDirty(item, fileName, DefaultStructuralEqualityComparer<T>());
        }

        public virtual bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer)
        {
            Ensure.IsValidFileName(fileName, "fileName");
            var fileInfo = GetFileInfo(fileName);
            return IsDirty(item, fileInfo, comparer);
        }

        public virtual bool IsDirty<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, "file");

            return IsDirty(item, file, DefaultStructuralEqualityComparer<T>());
        }

        public virtual bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            Ensure.NotNull(file, "file");

            VerifyDisposed();
            if (!Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("Cannot check IsDirty if not Setting.IsTrackingDirty");
            }
            return Tracker.IsDirty(item, file, comparer);
        }

        public bool CanRename<T>(string newName)
        {
            Ensure.IsValidFileName(newName, "newName");

            var fileInfo = GetFileInfo<T>();
            return CanRename(fileInfo, newName);
        }

        public void Rename<T>(string newName, bool owerWrite)
        {
            Ensure.IsValidFileName(newName, "newName");
            var fileInfo = GetFileInfo<T>();
            Rename(fileInfo, newName, owerWrite);
        }

        public bool CanRename(string oldName, string newName)
        {
            Ensure.IsValidFileName(oldName, "oldName");
            Ensure.IsValidFileName(newName, "newName");
            VerifyDisposed();
            var oldFile = GetFileInfo(oldName);
            var newFile = GetFileInfo(newName);
            return CanRename(oldFile, newFile);
        }

        public void Rename(string oldName, string newName, bool owerWrite)
        {
            Ensure.IsValidFileName(oldName, "oldName");
            Ensure.IsValidFileName(newName, "newName");
            VerifyDisposed();
            var oldFile = GetFileInfo(oldName);
            var newFile = GetFileInfo(newName);
            Rename(oldFile, newFile, owerWrite);
        }

        public bool CanRename(FileInfo oldName, string newName)
        {
            Ensure.NotNull(oldName, "oldName");
            Ensure.IsValidFileName(newName, "newName");
            VerifyDisposed();
            var newFile = GetFileInfo(newName);
            return CanRename(oldName, newFile);
        }

        public void Rename(FileInfo oldName, string newName, bool owerWrite)
        {
            Ensure.Exists(oldName, "oldName");
            Ensure.IsValidFileName(newName, "newName");
            VerifyDisposed();
            var newFile = GetFileInfo(newName);
            Rename(oldName, newFile, owerWrite);
        }

        public bool CanRename(FileInfo oldName, FileInfo newName)
        {
            Ensure.NotNull(oldName, "oldName");
            Ensure.NotNull(newName, "newName");
            VerifyDisposed();
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
            if (_cache.ContainsKey(newName.FullName))
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

        public void Rename(FileInfo oldName, FileInfo newName, bool owerWrite)
        {
            Ensure.NotNull(oldName, "oldName");
            Ensure.NotNull(newName, "newName");
            VerifyDisposed();
            oldName.Rename(newName, owerWrite);
            if (Backuper != null)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newName.Name);
                Backuper.Rename(oldName, fileNameWithoutExtension, owerWrite);
            }
            _cache.ChangeKey(oldName.FullName, newName.FullName, owerWrite);
            if (Settings.IsTrackingDirty && Tracker != null)
            {
                Tracker.Rename(oldName, newName, owerWrite);
            }
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
            Ensure.NotNull(item, "item");
            VerifyDisposed();
            return CloneCore(item);
        }

        public virtual T CloneCore<T>(T item)
        {
            Ensure.NotNull(item, "item");
            VerifyDisposed();
            using (var stream = ToStream(item))
            {
                return FromStream<T>(stream);
            }
        }

        public void ClearCache()
        {
            VerifyDisposed();
            _cache.Clear();
        }

        public void RemoveFromCache<T>(T item)
        {
            VerifyDisposed();
            var matches = _cache.Where(kvp => kvp.Value != null && ReferenceEquals(kvp.Value.Target, item))
                                .Select(x => x.Key)
                                .ToArray();
            foreach (var key in matches)
            {
                WeakReference temp;
                _cache.TryRemove(key, out temp);
            }
        }

        public void ClearTrackerCache()
        {
            VerifyDisposed();
            Tracker.ClearCache();
        }

        public void RemoveFromDirtyTracker<T>(T item)
        {
            VerifyDisposed();
            var tracker = Tracker;
            if (tracker == null)
            {
                return;
            }
            var fileInfo = GetFileInfo<T>();
            tracker.RemoveFromCache(fileInfo);
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
            CacheCore(item, file);
        }

        protected void CacheCore<T>(T item, FileInfo file)
        {
            VerifyDisposed();
            WeakReference cached;
            if (_cache.TryGetValue(file.FullName, out cached))
            {
                if (!ReferenceEquals(item, cached.Target))
                {
                    throw new InvalidOperationException("Trying to save a different instance than the cached");
                }
            }
            else
            {
                _cache.TryAdd(file.FullName, new WeakReference(item));
            }
        }

        private bool PrepareForSaveCore<T>(T item, FileInfo file)
        {
            if (Settings.IsCaching)
            {
                Cache(item, file);
            }

            if (Settings.IsTrackingDirty)
            {
                Tracker.TrackOrUpdate(file, item);
            }

            return Backuper.TryBackup(file);
        }
    }
}
