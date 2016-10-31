#pragma warning disable 1573
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
        public SingletonRepository(JsonSerializerSettings jsonSettings)
            : this(Directories.Default, jsonSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// It will use XmlRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public SingletonRepository(PathAndSpecialFolder directory)
            : this(directory.CreateDirectoryInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// It will use JsonRepositorySettings.DefaultFor(directory) as settings.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public SingletonRepository(DirectoryInfo directory)
            : base(directory, () => CreateDefaultSettings(directory), Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// It will use  JsonRepositorySettings.DefaultFor(directory, jsonSettings) to create the setting if no settings file exists.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public SingletonRepository(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
            : base(directory, () => CreateDefaultSettings(directory, jsonSettings), Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public SingletonRepository(PathAndSpecialFolder directory, Func<RepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), settingsCreator, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public SingletonRepository(DirectoryInfo directory, Func<RepositorySettings> settingsCreator)
            : base(directory, settingsCreator, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public SingletonRepository(PathAndSpecialFolder directory, IBackuper backuper, Func<RepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), backuper, settingsCreator, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public SingletonRepository(DirectoryInfo directory, IBackuper backuper, Func<RepositorySettings> settingsCreator)
            : base(directory, backuper, settingsCreator, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// </summary>
        public SingletonRepository(RepositorySettings settings)
            : base(settings, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// </summary>
        public SingletonRepository(RepositorySettings settings, IBackuper backuper)
            : base(settings, backuper, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// </summary>
        public SingletonRepository(Core.RepositorySettings settings)
            : base(RepositorySettings.Create(settings), Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// </summary>
        public SingletonRepository(Core.RepositorySettings settings, IBackuper backuper)
            : base(RepositorySettings.Create(settings), backuper, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// </summary>
        public SingletonRepository(Core.RepositorySettings settings, JsonSerializerSettings jsonSettings, IBackuper backuper)
            : base(RepositorySettings.Create(settings, jsonSettings), backuper, Serialize<RepositorySettings>.Default)
        {
        }

        protected new static RepositorySettings CreateDefaultSettings(DirectoryInfo directory)
        {
            return RepositorySettings.Create(Core.SingletonRepository<Core.RepositorySettings>.CreateDefaultSettings(directory));
        }

        protected static RepositorySettings CreateDefaultSettings(DirectoryInfo directory, JsonSerializerSettings jsonSerializerSettings)
        {
            return RepositorySettings.Create(Core.SingletonRepository<Core.RepositorySettings>.CreateDefaultSettings(directory), jsonSerializerSettings);
        }
    }
}
