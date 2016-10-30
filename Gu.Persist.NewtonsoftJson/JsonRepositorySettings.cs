namespace Gu.Persist.NewtonsoftJson
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using Gu.Persist.Core;

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
            this.JsonSerializerSettings = CreateDefaultJsonSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositorySettings"/> class.
        /// </summary>
        public JsonRepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
            this.JsonSerializerSettings = CreateDefaultJsonSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositorySettings"/> class.
        /// </summary>
        public JsonRepositorySettings(
            DirectoryInfo directory,
            JsonSerializerSettings jsonSerializerSettings,
            bool isTrackingDirty,
            bool isCaching,
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : this(
                PathAndSpecialFolder.Create(directory),
                jsonSerializerSettings,
                isTrackingDirty,
                isCaching,
                saveNullDeletesFile,
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
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : this(
                PathAndSpecialFolder.Default,
                jsonSerializerSettings,
                isTrackingDirty,
                isCaching,
                saveNullDeletesFile,
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
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(
                directoryPath,
                isTrackingDirty,
                isCaching,
                saveNullDeletesFile,
                backupSettings,
                extension,
                tempExtension)
        {
            this.JsonSerializerSettings = jsonSerializerSettings;
        }

        /// <summary>
        /// The settings controlling json serialization.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; }

        /// <summary>
        /// Creates a <see cref="JsonRepositorySettings"/> for <paramref name="directory"/>
        /// Uses DefaultJsonSettings
        /// </summary>
        public static JsonRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return DefaultFor(directory, CreateDefaultJsonSettings());
        }

        /// <summary>
        /// Creates a <see cref="JsonRepositorySettings"/> for <paramref name="directory"/>
        /// </summary>
        public static JsonRepositorySettings DefaultFor(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
        {
            return new JsonRepositorySettings(PathAndSpecialFolder.Create(directory), jsonSettings, true, true, false, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }

        /// <summary>
        /// The <see cref="JsonSerializerSettings"/> that will be used if none are specified.
        /// </summary>
        public static JsonSerializerSettings CreateDefaultJsonSettings()
        {
            return new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                Formatting = Formatting.Indented,
                Culture = CultureInfo.InvariantCulture,
                FloatFormatHandling = FloatFormatHandling.DefaultValue,
                //Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
            };
        }
    }
}