namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Settings.Backup;
    using Gu.Settings.IO;

    public abstract class Repository : IRepository, IAsyncRepository, IAutoAsyncRepository, IAutoRepository, ICloner, IAutoSavingRepository, IFileNameRepository, IRepositoryWithSettings, IDisposable
    {
        private readonly ConcurrentDictionary<FileInfo, WeakReference> _cache = new ConcurrentDictionary<FileInfo, WeakReference>(FileInfoComparer.Default);
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

        public IRepositorySettings Settings { get; private set; }

        public virtual IDirtyTracker Tracker { get; private set; }

        public virtual IBackuper Backuper
        {
            get { return _backuper ?? NullBackuper.Default; }
            private set
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
            return GetFileInfoCore<T>();
        }

        protected FileInfo GetFileInfoCore<T>()
        {
            return FileHelper.CreateFileInfo<T>(Settings);
        }

        public virtual bool Exists<T>()
        {
            return ExistsCore<T>();
        }

        protected bool ExistsCore<T>()
        {
            var file = GetFileInfoCore<T>();
            return ExistsCore<T>(file);
        }

        public virtual bool Exists<T>(string fileName)
        {
            var fileInfo = FileHelper.CreateFileInfo(fileName, Settings);
            return Exists<T>(fileInfo);
        }

        public virtual bool Exists<T>(FileInfo file)
        {
            return ExistsCore<T>(file);
        }

        protected bool ExistsCore<T>(FileInfo file)
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
            var fileInfo = FileHelper.CreateFileInfo(fileName, Settings);
            return ReadAsync<T>(fileInfo);
        }

        public virtual async Task<T> ReadAsync<T>(FileInfo file)
        {
            VerifyDisposed();
            Ensure.NotNull(file,"file"); // not checking exists, framework exception is more familiar.
            if (Settings.IsCaching)
            {
                WeakReference cached;
                if (_cache.TryGetValue(file, out cached))
                {
                    return (T)cached.Target;
                }
            }

            var value = await FileHelper.ReadAsync(file, FromStream<T>);
            if (Settings.IsCaching)
            {
                _cache.TryAdd(file, new WeakReference(value));                
            }
            if (Settings.IsTrackingDirty)
            {
                Tracker.TrackOrUpdate(file, value);
            }
            return value;
        }

        public virtual T Read<T>()
        {
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
            var fileInfo = FileHelper.CreateFileInfo(fileName, Settings);
            return Read<T>(fileInfo);
        }

        public virtual T Read<T>(FileInfo file)
        {
            return ReadCore<T>(file);
        }

        protected T ReadCore<T>(FileInfo file)
        {
            VerifyDisposed();
            Ensure.NotNull(file, "file");
            if (Settings.IsCaching)
            {
                WeakReference cached;
                if (_cache.TryGetValue(file, out cached))
                {
                    return (T)cached.Target;
                }
            }

            var value = FileHelper.Read(file, FromStream<T>);
            if (Settings.IsCaching)
            {
                _cache.TryAdd(file, new WeakReference(value));
            }
            if (Settings.IsTrackingDirty)
            {
                Tracker.TrackOrUpdate(file, value);
            }
            return value;
        }

        public virtual T ReadOrCreate<T>(Func<T> creator)
        {
            return ReadOrCreateCore(creator);
        }

        protected T ReadOrCreateCore<T>(Func<T> creator)
        {
            var file = GetFileInfoCore<T>();
            return ReadOrCreateCore(file, creator);
        }

        public virtual T ReadOrCreate<T>(string fileName, Func<T> creator)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            var file = FileHelper.CreateFileInfo(fileName, Settings);
            return ReadOrCreate(file, creator);
        }

        public virtual T ReadOrCreate<T>(FileInfo file, Func<T> creator)
        {
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
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public virtual void Save<T>(T item)
        {
            SaveCore(item);
        }

        protected void SaveCore<T>(T item)
        {
            var file = GetFileInfoCore<T>();
            SaveCore(item, file);
        }

        public virtual void Save<T>(T item, string fileName)
        {
            var file = FileHelper.CreateFileInfo(fileName, Settings);
            Save(item, file);
        }

        public virtual void Save<T>(T item, FileInfo file)
        {
            SaveCore(item, file);
        }

        protected void SaveCore<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, "file");
            var tempFile = file.ChangeExtension(Settings.TempExtension);
            SaveCore(item, file, tempFile);
        }

        public virtual void Save<T>(T item, FileInfo file, FileInfo tempFile)
        {
            SaveCore(item, file, tempFile);
        }

        protected void SaveCore<T>(T item, FileInfo file, FileInfo tempFile)
        {
            VerifyDisposed();
            Ensure.NotNull(file, "file");
            Ensure.NotNull(tempFile, "tempFile");
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
            var file = GetFileInfo<T>();
            return SaveAsync(item, file);
        }

        public virtual Task SaveAsync<T>(T item, string fileName)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            var fileInfo = FileHelper.CreateFileInfo(fileName, Settings);
            return SaveAsync(item, fileInfo);
        }

        public virtual Task SaveAsync<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, "file");
            var tempFile = file.ChangeExtension(Settings.TempExtension);
            return SaveAsync(item, file, tempFile);
        }

        public virtual async Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile)
        {
            VerifyDisposed();
            Ensure.NotNull(file, "file");
            Ensure.NotNull(tempFile, "tempFile");
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
            return IsDirty(item, fileName, DefaultStructuralEqualityComparer<T>());
        }

        public virtual bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            var fileInfo = FileHelper.CreateFileInfo(fileName, Settings);
            return IsDirty(item, fileInfo, comparer);
        }

        public virtual bool IsDirty<T>(T item, FileInfo file)
        {
            return IsDirty(item, file, DefaultStructuralEqualityComparer<T>());
        }

        public virtual bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            VerifyDisposed();
            if (!Settings.IsTrackingDirty)
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
            return CloneCore(item);
        }

        public virtual T CloneCore<T>(T item)
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
            CacheCore(item, file);
        }

        protected void CacheCore<T>(T item, FileInfo file)
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
