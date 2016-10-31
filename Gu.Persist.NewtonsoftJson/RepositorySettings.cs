namespace Gu.Persist.NewtonsoftJson
{
    using System.Globalization;
    using Gu.Persist.Core;
    using Newtonsoft.Json;

    /// <summary>
    /// Settings used in <see cref="SingletonRepository"/>
    /// </summary>
    public class RepositorySettings : Core.RepositorySettings, IJsonRepositorySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// </summary>
        [JsonConstructor]
        public RepositorySettings(
            PathAndSpecialFolder directory,
            JsonSerializerSettings jsonSerializerSettings,
            bool isTrackingDirty,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(
                directory,
                isTrackingDirty,
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
                           ////Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
                       };
        }

        public static RepositorySettings Create(Core.RepositorySettings settings)
        {
            return Create(settings, CreateDefaultJsonSettings());
        }

        public static RepositorySettings Create(Core.RepositorySettings settings, JsonSerializerSettings jsonSettings)
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