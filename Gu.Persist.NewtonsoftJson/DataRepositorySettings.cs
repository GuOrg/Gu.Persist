namespace Gu.Persist.NewtonsoftJson
{
    using Gu.Persist.Core;

    using Newtonsoft.Json;

    /// <summary>
    /// Settings used in <see cref="DataRepository"/>
    /// </summary>
    public class DataRepositorySettings : Core.DataRepositorySettings, IJsonRepositorySetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositorySettings"/> class.
        /// </summary>
        [JsonConstructor]
        public DataRepositorySettings(
            string directory,
            JsonSerializerSettings jsonSerializerSettings,
            bool isTrackingDirty,
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
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