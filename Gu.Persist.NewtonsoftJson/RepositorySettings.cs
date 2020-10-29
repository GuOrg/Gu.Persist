namespace Gu.Persist.NewtonsoftJson
{
    using System.Globalization;
    using Gu.Persist.Core;
    using Newtonsoft.Json;

    /// <summary>
    /// Settings used in <see cref="SingletonRepository"/>.
    /// </summary>
    public class RepositorySettings : Core.RepositorySettings, IJsonRepositorySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// </summary>
        /// <param name="directory">The <see cref="string"/>.</param>
        /// <param name="jsonSerializerSettings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="isTrackingDirty">Configures if the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes.</param>
        /// <param name="backupSettings">The <see cref="BackupSettings"/>.</param>
        /// <param name="extension">The file extension..</param>
        /// <param name="tempExtension">The temp file extension. Files are first written to temp files then changed extension for atomic writes.</param>
        [JsonConstructor]
        public RepositorySettings(
            string directory,
            JsonSerializerSettings jsonSerializerSettings,
            bool isTrackingDirty,
            BackupSettings? backupSettings,
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
        /// Gets the settings controlling JSON serialization.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; }

        /// <summary>
        /// The <see cref="JsonSerializerSettings"/> that will be used if none are specified.
        /// </summary>
        /// <returns>An instance of <see cref="JsonSerializerSettings"/>.</returns>
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