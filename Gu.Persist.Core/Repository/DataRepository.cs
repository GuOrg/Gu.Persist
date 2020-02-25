namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    /// <summary>
    /// A repository that does not cache reads nor track dirty.
    /// </summary>
    /// <typeparam name="TSetting">The type of settings.</typeparam>
    public abstract class DataRepository<TSetting> : Repository<TSetting>, IDataRepository
        where TSetting : IDataRepositorySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with default settings.
        /// If the directory contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        /// <param name="serialize">Serialization logic.</param>
        protected DataRepository(Func<TSetting> settingsCreator, Serialize<TSetting> serialize)
            : base(settingsCreator, serialize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository{TSetting}"/> class.
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
        protected DataRepository(Func<TSetting> settingsCreator, IBackuper backuper, Serialize<TSetting> serialize)
            : base(settingsCreator, backuper, serialize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">Setting controlling behavior.</param>
        /// <param name="serialize">Serialization logic.</param>
        protected DataRepository(TSetting settings, Serialize<TSetting> serialize)
            : this(settings, Backup.Backuper.Create(settings.BackupSettings), serialize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">Setting controlling behavior.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="serialize">Serialization logic.</param>
        protected DataRepository(TSetting settings, IBackuper backuper, Serialize<TSetting> serialize)
            : base(settings, backuper, serialize)
        {
        }

        /// <inheritdoc/>
        public virtual void Delete<T>(bool deleteBackups)
        {
            var file = this.GetFileInfo<T>();
            this.Delete(file, deleteBackups);
        }

        /// <inheritdoc/>
        public virtual void Delete(string fileName, bool deleteBackups)
        {
            var file = this.GetFileInfoCore(fileName);
            this.Delete(file, deleteBackups);
        }

        /// <inheritdoc/>
        public virtual void Delete(FileInfo file, bool deleteBackups)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            file.Delete();
            file.DeleteSoftDeleteFileFor();
            if (deleteBackups)
            {
                this.DeleteBackups(file);
            }
        }

        /// <inheritdoc/>
        protected override void EnsureCanSave<T>(FileInfo file, T item)
        {
            if (!this.Settings.SaveNullDeletesFile && item is null)
            {
                throw new ArgumentNullException($"Cannot save null when Settings.SaveNullDeletesFile is false.");
            }
        }
    }
}