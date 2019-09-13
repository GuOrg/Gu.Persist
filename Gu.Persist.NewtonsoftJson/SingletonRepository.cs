namespace Gu.Persist.NewtonsoftJson
{
    using System;
    using System.IO;

    using Gu.Persist.Core;

    using Newtonsoft.Json;

    /// <summary>
    /// A repository reading and saving files using <see cref="JsonSerializer"/>
    /// This repository keeps a cache of all saves and reads an manages singleton instances.
    /// </summary>
    public class SingletonRepository : SingletonRepository<RepositorySettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// Uses <see cref="Directories.Default"/>
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public SingletonRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// It will use JsonRepositorySettings.DefaultFor(Directories.Default, jsonSettings)) as settings.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        /// <param name="jsonSettings">The <see cref="JsonSerializerSettings"/>.</param>
        public SingletonRepository(JsonSerializerSettings jsonSettings)
            : this(Directories.Default, jsonSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// It will use JsonRepositorySettings.DefaultFor(directory) as settings.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo"/>.</param>
        public SingletonRepository(DirectoryInfo directory)
            : base(() => CreateDefaultSettings(directory), Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// It will use  JsonRepositorySettings.DefaultFor(directory, jsonSettings) to create the setting if no settings file exists.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo"/>.</param>
        /// <param name="jsonSettings">The <see cref="JsonSerializerSettings"/>.</param>
        public SingletonRepository(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
            : base(() => CreateDefaultSettings(directory, jsonSettings), Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// If the directory contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        public SingletonRepository(Func<RepositorySettings> settingsCreator)
            : base(settingsCreator, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// If the directory contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        public SingletonRepository(Func<RepositorySettings> settingsCreator, IBackuper backuper)
            : base(settingsCreator, backuper, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="RepositorySettings"/>.</param>
        public SingletonRepository(RepositorySettings settings)
            : base(settings, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="RepositorySettings"/>.</param>
        /// <param name="backuper">The <see cref="IBackuper"/>.</param>
        public SingletonRepository(RepositorySettings settings, IBackuper backuper)
            : base(settings, backuper, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="Core.RepositorySettings"/>.</param>
        /// <param name="jsonSettings">The <see cref="JsonSerializerSettings"/>.</param>
        public SingletonRepository(Core.RepositorySettings settings, JsonSerializerSettings jsonSettings)
            : base(CreateSettings(settings ?? throw new ArgumentNullException(nameof(settings)), jsonSettings), Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="Core.RepositorySettings"/>.</param>
        /// <param name="jsonSettings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="backuper">The <see cref="IBackuper"/>.</param>
        public SingletonRepository(Core.RepositorySettings settings, JsonSerializerSettings jsonSettings, IBackuper backuper)
            : base(CreateSettings(settings ?? throw new ArgumentNullException(nameof(settings)), jsonSettings), backuper, Serialize<RepositorySettings>.Default)
        {
        }

        private static RepositorySettings CreateDefaultSettings(DirectoryInfo directory)
        {
            return CreateSettings(Default.RepositorySettings(directory));
        }

        private static RepositorySettings CreateDefaultSettings(DirectoryInfo directory, JsonSerializerSettings jsonSerializerSettings)
        {
            return CreateSettings(CreateSettings(Default.RepositorySettings(directory), jsonSerializerSettings));
        }

        private static RepositorySettings CreateSettings(Core.RepositorySettings settings)
        {
            return CreateSettings(settings, RepositorySettings.CreateDefaultJsonSettings());
        }

        private static RepositorySettings CreateSettings(Core.RepositorySettings settings, JsonSerializerSettings jsonSettings)
        {
            return new RepositorySettings(
                       settings.Directory,
                       jsonSettings,
                       settings.IsTrackingDirty,
                       settings.BackupSettings,
                       settings.Extension,
                       settings.TempExtension);
        }
    }
}
