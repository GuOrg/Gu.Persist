namespace Gu.Persist.Core
{
    using System;

    /// <summary>
    /// Settings for <see cref="DataRepository{TSetting}"/>.
    /// </summary>
    [Serializable]
    public class DataRepositorySettings : RepositorySettings, IDataRepositorySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositorySettings"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="isTrackingDirty">If true instances are tracked so that IsDirty can be queried later.</param>
        /// <param name="saveNullDeletesFile">If true passing null deletes the file.</param>
        /// <param name="backupSettings">The <see cref="BackupSettings"/>.</param>
        /// <param name="extension">The file extension.</param>
        /// <param name="tempExtension">The extension for temp files.</param>
        public DataRepositorySettings(
            string directory,
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