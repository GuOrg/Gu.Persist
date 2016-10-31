namespace Gu.Persist.Core
{
    using System;

    [Serializable]
    public class DataRepositorySettings : RepositorySettings, IDataRepositorySettings
    {
        public DataRepositorySettings(
            PathAndSpecialFolder directory,
            bool isTrackingDirty,
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, backupSettings, extension, tempExtension)
        {
            this.SaveNullDeletesFile = saveNullDeletesFile;
        }

        /// <inheritdoc/>
        public bool SaveNullDeletesFile { get; }
    }
}