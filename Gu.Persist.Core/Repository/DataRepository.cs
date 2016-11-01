namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    public abstract class DataRepository<TSetting> : Repository<TSetting>
        where TSetting : IDataRepositorySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with default settings.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where the repository reads and saves files.</param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        /// <param name="serialize">Serialization logic.</param>
        protected DataRepository(DirectoryInfo directory, Func<TSetting> settingsCreator, Serialize<TSetting> serialize)
            : base(directory, settingsCreator, serialize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository{TSetting}"/> class.
        /// Creates a new <see cref="Repository{TSetting}"/> with default settings.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where the repository reads and saves files.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        /// <param name="serialize">Serialization logic.</param>
        protected DataRepository(DirectoryInfo directory, IBackuper backuper, Func<TSetting> settingsCreator, Serialize<TSetting> serialize)
            : base(directory, backuper, settingsCreator, serialize)
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

        protected static DataRepositorySettings CreateDefaultSettings(DirectoryInfo directory)
        {
            return new DataRepositorySettings(
                       PathAndSpecialFolder.Create(directory),
                       false,
                       false,
                       BackupSettings.Create(directory));
        }

        protected override void EnsureCanSave<T>(FileInfo file, T item)
        {
            if (!this.Settings.SaveNullDeletesFile && item == null)
            {
                throw new ArgumentNullException($"Cannot save null when Settings.SaveNullDeletesFile is false.");
            }
        }
    }
}