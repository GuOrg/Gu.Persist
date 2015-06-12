namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public abstract class Repository : IRepository, IAsyncRepository, IAutoAsyncRepository, IAutoRepository, ICloner, IDirtyTracker, IAutoSavingRepository
    {
        private readonly ConcurrentDictionary<string, FileInfo> _fileNamesMap = new ConcurrentDictionary<string, FileInfo>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<FileInfo, IFileInfos> _fileInfosMap = new ConcurrentDictionary<FileInfo, IFileInfos>(FileInfoComparer.Default);
        private readonly ConcurrentDictionary<FileInfo, WeakReference> _cache = new ConcurrentDictionary<FileInfo, WeakReference>(FileInfoComparer.Default);

        protected Repository(DirectoryInfo directory)
            : this(new RepositorySetting(true,true,directory))
        {
        }

        protected Repository(IRepositorySetting setting)
        {
            Setting = setting;
            FileHelper.CreateDirectoryIfNotExists(setting.Directory);
            if (Setting.IsTrackingDirty)
            {
                DirtyTracker = new DirtyTracker(this);
            }
        }

        public IRepositorySetting Setting { get; private set; }

        public DirtyTracker DirtyTracker { get; private set; }

        public bool Exists<T>()
        {
            return Exists<T>(typeof(T).Name);
        }

        public bool Exists<T>(string fileName)
        {
            var fileInfo = CreateFileInfo(fileName);
            return Exists<T>(fileInfo);
        }

        public bool Exists<T>(FileInfo file)
        {
            file.Refresh();
            return file.Exists;
        }

        public Task<T> ReadAsync<T>()
        {
            return ReadAsync<T>(typeof(T).Name);
        }

        public Task<T> ReadAsync<T>(string fileName)
        {
            var fileInfo = CreateFileInfo(fileName);
            return ReadAsync<T>(fileInfo);
        }

        public async Task<T> ReadAsync<T>(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            WeakReference cached;
            if (_cache.TryGetValue(file, out cached))
            {
                return (T)cached.Target;
            }
            var value = await FileHelper.ReadAsync(file, FromStream<T>);
            _cache.TryAdd(file, new WeakReference(value));
            return value;
        }

        public T Read<T>()
        {
            return Read<T>(typeof(T).Name);
        }

        /// <summary>
        /// Reads from file the first time. After that it returns returns cached value (singleton).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Optional if blank a file with the name of the class is read.</param>
        /// <returns></returns>
        public T Read<T>(string fileName)
        {
            var fileInfo = CreateFileInfo(fileName);
            return Read<T>(fileInfo);
        }

        public T Read<T>(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            WeakReference cached;
            if (_cache.TryGetValue(file, out cached))
            {
                return (T)cached.Target;
            }
            var value = FileHelper.Read(file, FromStream<T>);
            _cache.TryAdd(file, new WeakReference(value));
            return value;
        }

        /// <summary>
        /// Saves to a file named typeof(T).Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Save<T>(T item)
        {
            Save(item, typeof(T).Name);
        }

        public void Save<T>(T item, string fileName)
        {
            var fileInfo = CreateFileInfo(fileName);
            Save(item, fileInfo);
        }

        public void Save<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, "file");
            var fileInfos = GetFileInfos(file);
            Save(item, fileInfos);
        }

        public virtual void Save<T>(T item, IFileInfos fileInfos)
        {
            Ensure.NotNull(fileInfos, "fileInfos");
            Ensure.NotNull(fileInfos.File, "fileInfos");
            Cache(item, fileInfos);
            if (Setting.IsTrackingDirty)
            {
                DirtyTracker.TrackOrUpdate(fileInfos.File, item);
            }
            FileHelper.SemaphoreSlim.Wait();
            try
            {
                if (Setting.CreateBackupOnSave)
                {
                    FileHelper.Backup(fileInfos, false);
                }
                try
                {
                    if (item == null)
                    {
                        FileHelper.Delete(fileInfos.File);
                    }
                    else
                    {
                        using (var stream = ToStream(item))
                        {
                            FileHelper.Save(fileInfos.TempFile, stream, false);
                        }
                        fileInfos.TempFile.MoveTo(fileInfos.File);
                    }
                }
                catch (Exception)
                {
                    if (Setting.CreateBackupOnSave)
                    {
                        FileHelper.Restore(fileInfos, false);
                    }
                    throw;
                }
            }
            finally
            {
                FileHelper.SemaphoreSlim.Release();
            }
        }

        public Task SaveAsync<T>(T item)
        {
            return SaveAsync(item, typeof(T).Name);
        }

        public Task SaveAsync<T>(T item, string fileName)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            var fileInfo = CreateFileInfo(fileName);
            return SaveAsync(item, fileInfo);
        }

        public Task SaveAsync<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, "file");
            var fileInfos = GetFileInfos(file);
            return SaveAsync(item, fileInfos);
        }

        public async Task SaveAsync<T>(T item, IFileInfos fileInfos)
        {
            Ensure.NotNull(fileInfos, "fileInfos");
            Ensure.NotNull(fileInfos.File, "fileInfos");
            Cache(item, fileInfos);
            if (Setting.IsTrackingDirty)
            {
                DirtyTracker.TrackOrUpdate(fileInfos.File, item);
            }
            await FileHelper.SemaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                if (Setting.CreateBackupOnSave)
                {
                    FileHelper.Backup(fileInfos, false);
                }
                try
                {
                    if (item == null)
                    {
                        FileHelper.Delete(fileInfos.File);
                    }
                    else
                    {
                        using (var stream = ToStream(item))
                        {
                            await FileHelper.SaveAsync(fileInfos.TempFile, stream, false)
                                            .ConfigureAwait(false);
                        }
                        fileInfos.TempFile.MoveTo(fileInfos.File);
                    }
                }
                catch (Exception)
                {
                    if (Setting.CreateBackupOnSave)
                    {
                        FileHelper.Restore(fileInfos, false);
                    }
                    throw;
                }
            }
            finally
            {
                FileHelper.SemaphoreSlim.Release();
            }
        }

        public bool IsDirty<T>(T item)
        {
            return IsDirty(item, EqualityComparer<T>.Default);
        }

        public bool IsDirty<T>(T item, IEqualityComparer<T> comparer)
        {
            return IsDirty(item, typeof(T).Name, comparer);
        }

        public bool IsDirty<T>(T item, string fileName)
        {
            return IsDirty(item, fileName, EqualityComparer<T>.Default);
        }

        public bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            var fileInfo = CreateFileInfo(fileName);
            return IsDirty(item, fileInfo);
        }

        public bool IsDirty<T>(T item, FileInfo file)
        {
            return IsDirty(item, file, EqualityComparer<T>.Default);
        }

        public bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            if (!Setting.IsTrackingDirty)
            {
                throw new InvalidOperationException("Cannot check IsDirty if not Setting.IsTrackingDirty");
            }
            return DirtyTracker.IsDirty(file, item, comparer);
        }

        protected abstract T FromStream<T>(Stream stream);

        protected abstract Stream ToStream<T>(T item);

        public virtual T Clone<T>(T item)
        {
            using (var stream = ToStream(item))
            {
                return FromStream<T>(stream);
            }
        }

        protected virtual void Cache<T>(T item, IFileInfos fileInfos)
        {
            WeakReference cached;
            if (_cache.TryGetValue(fileInfos.File, out cached))
            {
                if (!ReferenceEquals(item, cached.Target))
                {
                    throw new InvalidOperationException("Trying to save a different instance than the cached");
                }
            }
            else
            {
                _cache.TryAdd(fileInfos.File, new WeakReference(item));
            }
        }

        private FileInfo CreateFileInfo(string fileName)
        {
            Ensure.NotNull(fileName, "fileName");
            FileInfo fileInfo;
            if (_fileNamesMap.TryGetValue(fileName, out fileInfo))
            {
                return fileInfo;
            }
            fileInfo = FileHelper.CreateFileInfo(Setting.Directory, fileName, Setting.Extension);
            _fileNamesMap.TryAdd(fileName, fileInfo);
            return fileInfo;
        }

        private IFileInfos GetFileInfos(FileInfo file)
        {
            if (Setting.CreateBackupOnSave)
            {
                return _fileInfosMap.GetOrAdd(
                    file,
                    x => FileInfos.CreateFileInfos(file, Setting.TempExtension, Setting.BackupExtension));
            }
            return _fileInfosMap.GetOrAdd(file, x => FileInfos.CreateFileInfos(file, Setting.TempExtension, null));
        }
    }
}
