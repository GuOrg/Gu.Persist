namespace Gu.Settings.NewtonsoftJson
{
    using System.Globalization;
    using System.IO;

    using Gu.Settings.Core;

    using Newtonsoft.Json;

    /// <summary>
    /// Settings used in <see cref="JsonRepository"/>
    /// </summary>
    public class JsonRepositorySettings : RepositorySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositorySettings"/> class.
        /// </summary>
        public JsonRepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
            this.JsonSerializerSettings = DefaultJsonSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositorySettings"/> class.
        /// </summary>
        public JsonRepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
            this.JsonSerializerSettings = DefaultJsonSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositorySettings"/> class.
        /// </summary>
        public JsonRepositorySettings(
            DirectoryInfo directory,
            JsonSerializerSettings jsonSerializerSettings,
            bool isTrackingDirty,
            bool isCaching,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : this(
                PathAndSpecialFolder.Create(directory),
                jsonSerializerSettings,
                isTrackingDirty,
                isCaching,
                backupSettings,
                extension,
                tempExtension)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositorySettings"/> class.
        /// </summary>
        public JsonRepositorySettings(
            JsonSerializerSettings jsonSerializerSettings,
            bool isTrackingDirty,
            bool isCaching,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : this(
                PathAndSpecialFolder.Default,
                jsonSerializerSettings,
                isTrackingDirty,
                isCaching,
                backupSettings,
                extension,
                tempExtension)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositorySettings"/> class.
        /// </summary>
        [JsonConstructor]
        public JsonRepositorySettings(
            PathAndSpecialFolder directoryPath,
            JsonSerializerSettings jsonSerializerSettings,
            bool isTrackingDirty,
            bool isCaching,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(directoryPath, isTrackingDirty, isCaching, backupSettings, extension, tempExtension)
        {
            this.JsonSerializerSettings = jsonSerializerSettings;
        }

        /// <summary>
        /// The <see cref="JsonSerializerSettings"/> that will be used if none are specified.
        /// </summary>
        public static JsonSerializerSettings DefaultJsonSettings => new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error,
            Formatting = Formatting.Indented,
            Culture = CultureInfo.InvariantCulture,
            FloatFormatHandling = FloatFormatHandling.DefaultValue
        };

        /// <summary>
        /// The settings controlling json serialization.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; private set; }

        /// <summary>
        /// Creates a <see cref="JsonRepositorySettings"/> for <paramref name="directory"/>
        /// Uses DefaultJsonSettings
        /// </summary>
        public new static JsonRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return DefaultFor(directory, DefaultJsonSettings);
        }

        /// <summary>
        /// Creates a <see cref="JsonRepositorySettings"/> for <paramref name="directory"/>
        /// </summary>
        public static JsonRepositorySettings DefaultFor(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
        {
            return new JsonRepositorySettings(PathAndSpecialFolder.Create(directory), jsonSettings, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}