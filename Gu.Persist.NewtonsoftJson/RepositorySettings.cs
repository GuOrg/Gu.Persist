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
            string directory,
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
                       };
        }
    }
}