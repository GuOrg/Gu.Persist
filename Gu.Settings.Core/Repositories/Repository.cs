namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Settings.Core.Backup;

    public abstract class Repository<TSetting> : IRepository, IGenericAsyncRepository, IAsyncFileNameRepository, ICloner, IRepositoryWithSettings
        where TSetting : IRepositorySettings
    {
        private readonly object gate = new object();
        private readonly FileCache fileCache = new FileCache();
        [EditorBrowsable(EditorBrowsableState.Never)]
        private IBackuper backuper;

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
            this.Settings = settingsCreator();
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker = new DirtyTracker(this);
            }

            this.Backuper = Backup.Backuper.Create(this.Settings.BackupSettings); // creating temp for TryRestore in ReadOrCreate
            this.Settings = this.ReadOrCreateCore(() => (TSetting)this.Settings);
            this.Backuper = Backup.Backuper.Create(this.Settings.BackupSettings);
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
            this.Settings = settings;
            this.Backuper = backuper;
            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker = new DirtyTracker(this);
            }
        }

        /// <inheritdoc/>
        public IRepositorySettings Settings { get; }
       
        /// <inheritdoc/>
        public IDirtyTracker Tracker { get; }

        /// <inheritdoc/>
        public IBackuper Backuper
        {
            get { return this.backuper ?? NullBackuper.Default; }
            protected set
            {
                this.backuper = value;
            }
        }

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
        protected FileInfo GetFileInfoCore(string fileName)
        {
            var file = FileHelper.CreateFileInfo(fileName, this.Settings);
            return file;
        }

        /// <inheritdoc/>
        protected FileInfo GetFileInfoCore<T>()
        {
            return FileHelper.CreateFileInfo<T>(this.Settings);
        }

        /// <inheritdoc/>
        public virtual void Delete<T>(bool deleteBackups)
        {
            var file = this.GetFileInfo<T>();
            this.Delete(file, deleteBackups);
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups<T>()
        {
            var file = this.GetFileInfo<T>();
            this.DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual void Delete(string fileName, bool deleteBackups)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            this.Delete(file, deleteBackups);
        }

        /// <inheritdoc/>
        public virtual void DeleteBackups(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            this.DeleteBackups(file);
        }

        /// <inheritdoc/>
        public virtual void Delete(FileInfo file, bool deleteBackups)
        {
            Ensure.NotNull(file, nameof(file));
            file.Delete();
            file.DeleteSoftDeleteFileFor();
            if (deleteBackups)
            {
                this.DeleteBackups(file);
            }
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

        protected bool ExistsCore<T>()
        {
            var file = this.GetFileInfoCore<T>();
            return this.ExistsCore(file);
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

        protected bool ExistsCore(FileInfo file)
        {
            file.Refresh();
            return file.Exists;
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadAsync<T>()
        {
            var file = this.GetFileInfo<T>();
            return this.ReadAsync<T>(file);
        }

        /// <inheritdoc/>
        public virtual Task<MemoryStream> ReadStreamAsync<T>()
        {
            var file = this.GetFileInfo<T>();
            return this.ReadStreamAsync(file);
        }

        /// <inheritdoc/>
        public virtual Task<T> ReadAsync<T>(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = this.GetFileInfoCore(fileName);
            return this.ReadAsync<T>(fileInfo);
        }

        /// <inheritdoc/>
        public virtual Task<MemoryStream> ReadStreamAsync(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = this.GetFileInfoCore(fileName);
            return this.ReadStreamAsync(fileInfo);
        }

        /// <inheritdoc/>
        public virtual async Task<T> ReadAsync<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file)); // not checking exists, framework exception is more familiar.
            T value;
            if (this.Settings.IsCaching)
            {
                T cached;
                if (this.fileCache.TryGetValue(file.FullName, out cached))
                {
                    return cached;
                }

                // can't await  inside the lock. 
                // If there are many threads reading the same only the first is used
                // the other reads are wasted, can't think of anything better than this.
                value = await FileHelper.ReadAsync<T>(file, this.FromStream<T>).ConfigureAwait(false);

                lock (this.gate)
                {
                    if (this.fileCache.TryGetValue(file.FullName, out cached))
                    {
                        return cached;
                    }

                    this.fileCache.Add(file.FullName, value);
                }
            }
            else
            {
                value = await FileHelper.ReadAsync<T>(file, this.FromStream<T>).ConfigureAwait(false);
            }

            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker.Track(file.FullName, value);
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
            return this.ReadCore<T>();
        }

        /// <inheritdoc/>
        public virtual Stream ReadStream<T>()
        {
            var file = this.GetFileInfoCore<T>();
            return this.ReadStream(file);
        }

        protected T ReadCore<T>()
        {
            var file = this.GetFileInfoCore<T>();
            return this.ReadCore<T>(file);
        }

        /// <inheritdoc/>
        public virtual T Read<T>(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            return this.Read<T>(file);
        }

        /// <inheritdoc/>
        public virtual Stream ReadStream(string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            return this.ReadStream(file);
        }

        /// <inheritdoc/>
        public virtual T Read<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return this.ReadCore<T>(file);
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
            if (this.Settings.IsCaching)
            {
                T cached;
                if (this.fileCache.TryGetValue(file.FullName, out cached))
                {
                    return (T)cached;
                }

                lock (this.gate)
                {
                    if (this.fileCache.TryGetValue(file.FullName, out cached))
                    {
                        return (T)cached;
                    }

                    value = FileHelper.Read<T>(file, this.FromStream<T>);
                    this.fileCache.Add(file.FullName, value);
                }
            }
            else
            {
                value = FileHelper.Read<T>(file, this.FromStream<T>);
            }

            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker.Track(file.FullName, value);
            }

            return value;
        }

        /// <inheritdoc/>
        public virtual T ReadOrCreate<T>(Func<T> creator)
        {
            Ensure.NotNull(creator, nameof(creator));
            return this.ReadOrCreateCore(creator);
        }

        protected T ReadOrCreateCore<T>(Func<T> creator)
        {
            Ensure.NotNull(creator, nameof(creator));
            var file = this.GetFileInfoCore<T>();
            return this.ReadOrCreateCore(file, creator);
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

        protected T ReadOrCreateCore<T>(FileInfo file, Func<T> creator)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(creator, nameof(creator));
            T setting;
            if (this.ExistsCore<T>())
            {
                setting = this.ReadCore<T>();
            }
            else if (this.Backuper.TryRestore(file))
            {
                setting = this.ReadCore<T>();
            }
            else
            {
                setting = creator();
                this.SaveCore(setting);
            }

            return setting;
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item)
        {
            this.SaveCore(item);
        }

        /// <inheritdoc/>
        public virtual void SaveAndClose<T>(T item)
        {
            var file = this.GetFileInfoCore<T>();
            this.Save(item, file);
            this.RemoveFromCache(item);
            this.RemoveFromDirtyTracker(item);
        }

        protected void SaveCore<T>(T item)
        {
            var file = this.GetFileInfoCore<T>();
            this.SaveCore(item, file);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            this.Save(item, file);
        }

        /// <inheritdoc/>
        public virtual void SaveAndClose<T>(T item, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            this.Save(item, fileName);
            this.RemoveFromCache(item);
            this.RemoveFromDirtyTracker(item);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            this.SaveCore(item, file);
        }

        /// <inheritdoc/>
        public virtual void SaveAndClose<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            this.SaveCore(item, file);
            this.RemoveFromCache(item);
            this.RemoveFromDirtyTracker(item);
        }

        protected void SaveCore<T>(T item, FileInfo file)
        {
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            this.SaveCore(item, file, tempFile);
        }

        /// <inheritdoc/>
        public virtual void Save<T>(T item, FileInfo file, FileInfo tempFile)
        {
            Ensure.NotNull(file, nameof(file));
            this.SaveCore(item, file, tempFile);
        }

        /// <inheritdoc/>
        public virtual void SaveStream<T>(Stream stream)
        {
            var file = this.GetFileInfoCore<T>();
            this.SaveStream(stream, file);
        }

        /// <inheritdoc/>
        public virtual void SaveStream(Stream stream, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            this.SaveStream(stream, file);
        }

        /// <inheritdoc/>
        public virtual void SaveStream(Stream stream, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            this.SaveStreamCore(stream, file, tempFile);
        }

        /// <inheritdoc/>
        public virtual void SaveStream(Stream stream, FileInfo file, FileInfo tempFile)
        {
            Ensure.NotNull(file, nameof(file));
            this.SaveStreamCore(stream, file, tempFile);
        }

        protected void SaveCore<T>(T item, FileInfo file, FileInfo tempFile)
        {
            if (item == null)
            {
                this.SaveStreamCore(null, file, null);
                return;
            }

            this.CacheAndTrackCore(item, file);

            using (var stream = this.ToStream(item))
            {
                this.SaveStreamCore(stream, file, tempFile);
            }
        }

        protected void SaveStreamCore(Stream stream, FileInfo file, FileInfo tempFile)
        {
            if (stream == null)
            {
                FileHelper.HardDelete(file);
                return;
            }

            this.Backuper.BeforeSave(file);
            try
            {
                FileHelper.Save(tempFile, stream);
                tempFile.MoveTo(file);
                this.Backuper.AfterSuccessfulSave(file);
            }
            catch (Exception exception)
            {
                try
                {
                    this.Backuper.TryRestore(file);
                }
                catch (Exception restoreException)
                {
                    throw new RestoreException(exception, restoreException);
                }

                throw;
            }
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(T item)
        {
            var file = this.GetFileInfo<T>();
            return this.SaveAsync(item, file);
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(T item, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = this.GetFileInfoCore(fileName);
            return this.SaveAsync(item, fileInfo);
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            return this.SaveAsync(item, file, tempFile);
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

            this.CacheAndTrackCore(item, file);
            using (var stream = this.ToStream(item))
            {
                await this.SaveStreamAsync(stream, file, tempFile).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual Task SaveStreamAsync<T>(Stream stream)
        {
            Ensure.NotNull(stream, nameof(stream));
            var file = this.GetFileInfo<T>();
            return this.SaveStreamAsync(stream, file);
        }

        /// <inheritdoc/>
        public virtual Task SaveStreamAsync(Stream stream, string fileName)
        {
            Ensure.NotNull(stream, nameof(stream));
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var file = this.GetFileInfoCore(fileName);
            return this.SaveStreamAsync(stream, file);
        }

        /// <inheritdoc/>
        public virtual Task SaveStreamAsync(Stream stream, FileInfo file)
        {
            Ensure.NotNull(stream, nameof(stream));
            Ensure.NotNull(file, nameof(file));
            var tempFile = file.WithNewExtension(this.Settings.TempExtension);
            return this.SaveStreamAsync(stream, file, tempFile);
        }

        /// <inheritdoc/>
        public virtual async Task SaveStreamAsync(Stream stream, FileInfo file, FileInfo tempFile)
        {
            this.Backuper.BeforeSave(file);
            try
            {
                await tempFile.SaveAsync(stream).ConfigureAwait(false);
                tempFile.MoveTo(file);
                this.Backuper.AfterSuccessfulSave(file);
            }
            catch (Exception)
            {
                this.Backuper.TryRestore(file);
                throw;
            }
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item)
        {
            return this.IsDirty(item, this.DefaultStructuralEqualityComparer<T>());
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, IEqualityComparer<T> comparer)
        {
            var file = this.GetFileInfo<T>();
            return this.IsDirty(item, file, comparer);
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, string fileName)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            return this.IsDirty(item, fileName, this.DefaultStructuralEqualityComparer<T>());
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer)
        {
            Ensure.IsValidFileName(fileName, nameof(fileName));
            var fileInfo = this.GetFileInfoCore(fileName);
            return this.IsDirty(item, fileInfo, comparer);
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));

            return this.IsDirty(item, file, this.DefaultStructuralEqualityComparer<T>());
        }

        /// <inheritdoc/>
        public virtual bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            Ensure.NotNull(file, nameof(file));
            if (!this.Settings.IsTrackingDirty)
            {
                throw new InvalidOperationException("Cannot check IsDirty if not Setting.IsTrackingDirty");
            }

            return this.Tracker.IsDirty(item, file.FullName, comparer);
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
        public void Rename(string oldName, string newName, bool owerWrite)
        {
            Ensure.IsValidFileName(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var oldFile = this.GetFileInfoCore(oldName);
            var newFile = this.GetFileInfoCore(newName);
            this.Rename(oldFile, newFile, owerWrite);
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
        public void Rename(FileInfo oldName, string newName, bool owerWrite)
        {
            Ensure.Exists(oldName, nameof(oldName));
            Ensure.IsValidFileName(newName, nameof(newName));
            var newFile = this.GetFileInfoCore(newName);
            this.Rename(oldName, newFile, owerWrite);
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

            if (this.fileCache.ContainsKey(newName.FullName))
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
        public void Rename(FileInfo oldName, FileInfo newName, bool owerWrite)
        {
            Ensure.NotNull(oldName, nameof(oldName));
            Ensure.NotNull(newName, nameof(newName));
            oldName.Rename(newName, owerWrite);
            if (this.Backuper != null)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newName.Name);
                this.Backuper.Rename(oldName, fileNameWithoutExtension, owerWrite);
            }

            this.fileCache.ChangeKey(oldName.FullName, newName.FullName, owerWrite);
            if (this.Settings.IsTrackingDirty && this.Tracker != null)
            {
                this.Tracker.Rename(oldName.FullName, newName.FullName, owerWrite);
            }
        }

        /// <inheritdoc/>
        public virtual T Clone<T>(T item)
        {
            Ensure.NotNull(item, nameof(item));
            return this.CloneCore(item);
        }

        public virtual T CloneCore<T>(T item)
        {
            Ensure.NotNull(item, nameof(item));
            using (var stream = this.ToStream(item))
            {
                return this.FromStream<T>(stream);
            }
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

            var fileInfo = this.GetFileInfo<T>();
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
            this.CacheCore(item, file);
        }

        protected void CacheCore<T>(T item, FileInfo file)
        {
            T cached;
            if (this.fileCache.TryGetValue(file.FullName, out cached))
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

        private void CacheAndTrackCore<T>(T item, FileInfo file)
        {
            if (this.Settings.IsCaching)
            {
                this.Cache(item, file);
            }

            if (this.Settings.IsTrackingDirty)
            {
                this.Tracker.Track(file.FullName, item);
            }
        }
    }
}
