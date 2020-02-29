namespace Gu.Persist.NewtonsoftJson
{
    using Gu.Persist.Core;

    using Newtonsoft.Json;

    /// <summary>
    /// Settings used in <see cref="DataRepository"/>.
    /// </summary>
    public class DataRepositorySettings : Core.DataRepositorySettings, IJsonRepositorySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositorySettings"/> class.
        /// </summary>
        /// <param name="directory">The <see cref="string"/>.</param>
        /// <param name="jsonSerializerSettings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="saveNullDeletesFile">If saving null should throw or delete the file.</param>
        /// <param name="isTrackingDirty">Configures if the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes.</param>
        /// <param name="backupSettings">The <see cref="BackupSettings"/>.</param>
        /// <param name="extension">The file extension.</param>
        /// <param name="tempExtension">The temp file extension. Files are first written to temp files then changed extension for atomic writes.</param>
        [JsonConstructor]
        public DataRepositorySettings(
            string directory,
            JsonSerializerSettings jsonSerializerSettings,
            bool isTrackingDirty,
            bool saveNullDeletesFile,
            BackupSettings? backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, saveNullDeletesFile, backupSettings, extension, tempExtension)
        {
            this.JsonSerializerSettings = jsonSerializerSettings;
        }

        /// <summary>
        /// The settings controlling json serialization.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; }
    }
}