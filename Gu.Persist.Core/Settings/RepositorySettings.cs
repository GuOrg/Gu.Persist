namespace Gu.Persist.Core
{
    using System;

    /// <summary>
    /// A setting for <see cref="Repository{TSetting}"/>.
    /// </summary>
    [Serializable]
    public class RepositorySettings : FileSettings, IRepositorySettings
    {
        /// <summary>
        /// The default name of the backup directory.
        /// </summary>
        protected static readonly string DefaultBackupDirectoryName = "Backup";

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// </summary>
        /// <param name="directory">The <see cref="string"/>.</param>
        /// <param name="isTrackingDirty">Configures if the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes.</param>
        /// <param name="backupSettings">The <see cref="BackupSettings"/>.</param>
        /// <param name="extension">The file extension.</param>
        /// <param name="tempExtension">The temp file extension. Files are first written to temp files then changed extension for atomic writes.</param>
        public RepositorySettings(
            string directory,
            bool isTrackingDirty,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(directory, extension)
        {
            Ensure.NotNullOrEmpty(extension, nameof(extension));
            Ensure.NotNullOrEmpty(tempExtension, nameof(tempExtension));
            Ensure.NotNull(directory, nameof(directory));

            this.IsTrackingDirty = isTrackingDirty;
            this.BackupSettings = backupSettings;
            this.TempExtension = FileHelper.PrependDotIfMissing(tempExtension);
        }

        /// <summary>
        /// Gets the <see cref="BackupSettings"/>.
        /// </summary>
        public BackupSettings BackupSettings { get; }

        /// <summary>
        /// Gets the file extension used when saving files.
        /// On successful save the file extension is replaced.
        /// </summary>
        public string TempExtension { get; }

        /// <summary>
        /// Gets a value indicating whether the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes.
        /// </summary>
        public bool IsTrackingDirty { get; }
    }
}
