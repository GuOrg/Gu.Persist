namespace Gu.Persist.NewtonsoftJson
{
    using System;
    using System.IO;

    using Gu.Persist.Core;

    using Newtonsoft.Json;

    /// <summary>
    /// A repository reading and saving files using <see cref="JsonSerializer"/>.
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
        /// It will use DataRepository.DefaultFor(directory) as settings.
        /// </summary>
        /// <param name="directory">The directory where the repository reads and saves files.</param>
        public DataRepository(string directory)
            : this(new DirectoryInfo(directory))
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
        /// <param name="settings">The <see cref="DataRepositorySettings"/>.</param>
        public DataRepository(DataRepositorySettings settings)
            : base(settings, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="DataRepositorySettings"/>.</param>
        /// <param name="backuper">The <see cref="IBackuper"/>.</param>
        public DataRepository(DataRepositorySettings settings, IBackuper backuper)
            : base(settings, backuper, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// </summary>
        /// <param name="jsonSerializerSettings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="settings">The <see cref="Core.DataRepositorySettings"/>.</param>
        public DataRepository(Core.DataRepositorySettings settings, JsonSerializerSettings jsonSerializerSettings)
            : base(Create(settings, jsonSerializerSettings), Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="Core.DataRepositorySettings"/>.</param>
        /// <param name="jsonSerializerSettings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="backuper">The <see cref="IBackuper"/>.</param>
        public DataRepository(Core.DataRepositorySettings settings, JsonSerializerSettings jsonSerializerSettings, IBackuper backuper)
            : base(Create(settings, jsonSerializerSettings), backuper, Serialize<DataRepositorySettings>.Default)
        {
        }

        private static DataRepositorySettings CreateDefaultSettings(DirectoryInfo directory)
        {
            return Create(Default.DataRepositorySettings(directory));
        }

        private static DataRepositorySettings Create(Core.RepositorySettings settings)
        {
            return Create(settings, RepositorySettings.CreateDefaultJsonSettings());
        }

        private static DataRepositorySettings Create(IRepositorySettings settings, JsonSerializerSettings jsonSettings)
        {
            return new DataRepositorySettings(
                       settings.Directory,
                       jsonSettings,
                       settings.IsTrackingDirty,
                       (settings as IDataRepositorySettings)?.SaveNullDeletesFile == true,
                       settings.BackupSettings,
                       settings.Extension,
                       settings.TempExtension);
        }
    }
}