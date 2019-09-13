namespace Gu.Persist.SystemXml
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    /// <summary>
    /// A repository reading and saving files using <see cref="XmlSerializer"/>.
    /// </summary>
    public class DataRepository : DataRepository<DataRepositorySettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// Uses <see cref="Directories.Default"/>.
        /// </summary>
        public DataRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// It will use BinaryRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo"/>.</param>
        public DataRepository(DirectoryInfo directory)
            : base(() => CreateDefaultSettings(directory), Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// If the directory contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        public DataRepository(Func<DataRepositorySettings> settingsCreator)
            : base(settingsCreator, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// If the directory contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backup settings.
        /// </param>
        public DataRepository(Func<DataRepositorySettings> settingsCreator, IBackuper backuper)
            : base(settingsCreator, backuper, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="Core.IRepositorySettings"/>.</param>
        public DataRepository(IRepositorySettings settings)
            : base(Create(settings), Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="Core.IRepositorySettings"/>.</param>
        /// <param name="backuper">The <see cref="IBackuper"/>.</param>
        public DataRepository(IRepositorySettings settings, IBackuper backuper)
            : base(Create(settings), backuper, Serialize<DataRepositorySettings>.Default)
        {
        }

        private static DataRepositorySettings CreateDefaultSettings(DirectoryInfo directory)
        {
            return Create(Default.DataRepositorySettings(directory));
        }

        private static DataRepositorySettings Create(IRepositorySettings settings)
        {
            return new DataRepositorySettings(
                       settings.Directory,
                       settings.IsTrackingDirty,
                       (settings as IDataRepositorySettings)?.SaveNullDeletesFile == true,
                       settings.BackupSettings,
                       settings.Extension,
                       settings.TempExtension);
        }
    }
}