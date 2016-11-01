namespace Gu.Persist.SystemXml
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    /// <summary>
    /// A repository reading and saving files using <see cref="XmlSerializer"/>
    /// </summary>
    public class DataRepository : DataRepository<DataRepositorySettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// Uses <see cref="Directories.Default"/>
        /// </summary>
        public DataRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// It will use DataRepository.DefaultFor(directory) as settings.
        /// </summary>
        /// <param name="directory">The directory where the repository reads and saves files.</param>
        public DataRepository(PathAndSpecialFolder directory)
            : this(directory.CreateDirectoryInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// It will use BinaryRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public DataRepository(DirectoryInfo directory)
            : base(directory, () => CreateDefaultSettings(directory), Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where the repository reads and saves files.</param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public DataRepository(PathAndSpecialFolder directory, Func<DataRepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), settingsCreator, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where the repository reads and saves files.</param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public DataRepository(DirectoryInfo directory, Func<DataRepositorySettings> settingsCreator)
            : base(directory, settingsCreator, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public DataRepository(PathAndSpecialFolder directory, IBackuper backuper, Func<DataRepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), backuper, settingsCreator, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public DataRepository(DirectoryInfo directory, IBackuper backuper, Func<DataRepositorySettings> settingsCreator)
            : base(directory, backuper, settingsCreator, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// </summary>
        public DataRepository(Core.DataRepositorySettings settings)
            : base(Create(settings), Serialize<DataRepositorySettings>.Default)
        {
        }

        protected static DataRepositorySettings CreateDefaultSettings(DirectoryInfo directory)
        {
            return Create(Default.DataRepositorySettings(directory));
        }

        protected static DataRepositorySettings Create(IRepositorySettings settings)
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