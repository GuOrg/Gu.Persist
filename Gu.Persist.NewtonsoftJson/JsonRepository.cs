#pragma warning disable 1573
namespace Gu.Persist.NewtonsoftJson
{
    using System;
    using System.IO;

    using Gu.Persist.Core;

    using Newtonsoft.Json;

    /// <summary>
    /// A repository reading and saving files using <see cref="JsonSerializer"/>
    /// </summary>
    public class JsonRepository : Repository<JsonRepositorySettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// Uses <see cref="Directories.Default"/>
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public JsonRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// It will use JsonRepositorySettings.DefaultFor(Directories.Default, jsonSettings)) as settings.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public JsonRepository(JsonSerializerSettings jsonSettings)
            : base(Directories.Default, () => JsonRepositorySettings.DefaultFor(Directories.Default, jsonSettings), JsonSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// It will use XmlRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public JsonRepository(PathAndSpecialFolder directory)
            : this(directory.CreateDirectoryInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// It will use JsonRepositorySettings.DefaultFor(directory) as settings.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public JsonRepository(DirectoryInfo directory)
            : base(directory, () => JsonRepositorySettings.DefaultFor(directory), JsonSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// It will use  JsonRepositorySettings.DefaultFor(directory, jsonSettings) to create the setting if no settings file exists.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public JsonRepository(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
            : base(directory, () => JsonRepositorySettings.DefaultFor(directory, jsonSettings), JsonSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public JsonRepository(PathAndSpecialFolder directory, Func<JsonRepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), settingsCreator, JsonSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public JsonRepository(DirectoryInfo directory, Func<JsonRepositorySettings> settingsCreator)
            : base(directory, settingsCreator, JsonSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public JsonRepository(PathAndSpecialFolder directory, IBackuper backuper, Func<JsonRepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), backuper, settingsCreator, JsonSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public JsonRepository(DirectoryInfo directory, IBackuper backuper, Func<JsonRepositorySettings> settingsCreator)
            : base(directory, backuper, settingsCreator, JsonSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// </summary>
        public JsonRepository(JsonRepositorySettings settings)
            : base(settings, JsonSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// </summary>
        public JsonRepository(JsonRepositorySettings settings, IBackuper backuper)
            : base(settings, backuper, JsonSerialize.Default)
        {
        }

        /// <summary>
        /// The settings used by the repository.
        /// </summary>
        public new JsonRepositorySettings Settings => base.Settings;
    }
}
