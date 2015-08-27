namespace Gu.Settings.NewtonsoftJson
{
    using System.Globalization;
    using System.IO;

    using Gu.Settings.Core;

    using Newtonsoft.Json;

    public class JsonRepositorySettings : RepositorySettings
    {
        public static readonly JsonSerializerSettings DefaultJsonSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error,
            Formatting = Formatting.Indented,
            Culture = CultureInfo.InvariantCulture,
            FloatFormatHandling = FloatFormatHandling.DefaultValue
        };

        public JsonRepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
            JsonSerializerSettings = DefaultJsonSettings;
        }

        public JsonRepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
            JsonSerializerSettings = DefaultJsonSettings;
        }

        [JsonConstructor]
        public JsonRepositorySettings(
            DirectoryInfo directory,
            JsonSerializerSettings jsonSerializerSettings,
            bool isTrackingDirty,
            bool isCaching,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, isCaching, backupSettings, extension, tempExtension)
        {
            JsonSerializerSettings = jsonSerializerSettings;
        }

        public static new JsonRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return DefaultFor(directory, DefaultJsonSettings);
        }

        public static JsonRepositorySettings DefaultFor(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
        {
            return new JsonRepositorySettings(directory, jsonSettings, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(BackupDirectoryName)));
        }

        public JsonSerializerSettings JsonSerializerSettings { get; private set; }
    }
}