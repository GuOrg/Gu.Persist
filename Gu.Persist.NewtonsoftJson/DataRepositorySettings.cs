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
            PathAndSpecialFolder directory,
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

        public static DataRepositorySettings Create(Core.RepositorySettings settings)
        {
            return Create(settings, RepositorySettings.CreateDefaultJsonSettings());
        }

        public static DataRepositorySettings Create(Core.RepositorySettings settings, JsonSerializerSettings jsonSettings)
        {
            return new DataRepositorySettings(
                       settings.Directory,
                       jsonSettings,
                       settings.IsTrackingDirty,
                       (settings as IDataRepositorySettings)?.SaveNullDeletesFile == true,
                       settings.BackupSettings,
                       settings.Extension,
                       settings.TempExtension);
        }
    }
}