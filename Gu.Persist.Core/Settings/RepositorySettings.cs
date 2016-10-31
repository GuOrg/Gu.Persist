namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    [Serializable]
    public class RepositorySettings : FileSettings, IRepositorySettings
    {
        protected static readonly string DefaultBackupDirectoryName = "Backup";

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// </summary>
        public RepositorySettings(DirectoryInfo directory)
            : this(directory, BackupSettings.Create(directory))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// </summary>
        public RepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : this(PathAndSpecialFolder.Create(directory), true, false, backupSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// </summary>
        public RepositorySettings(
            PathAndSpecialFolder directory,
            bool isTrackingDirty,
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(directory, extension)
        {
            Ensure.NotNullOrEmpty(extension, nameof(extension));
            Ensure.NotNullOrEmpty(tempExtension, nameof(tempExtension));
            Ensure.NotNull(directory, nameof(directory));

            this.IsTrackingDirty = isTrackingDirty;
            this.SaveNullDeletesFile = saveNullDeletesFile;
            this.BackupSettings = backupSettings;
            this.TempExtension = FileHelper.PrependDotIfMissing(tempExtension);
        }

        protected RepositorySettings()
        {
        }

        public BackupSettings BackupSettings { get; }

        /// <summary>
        /// Gets or sets the file extension used when saving files.
        /// On successful save the file extension is replaced.
        /// </summary>
        public string TempExtension { get; }

        /// <summary>
        /// Gets or sets if the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes
        /// </summary>
        public bool IsTrackingDirty { get; }

        /// <summary>
        /// If true calling save with null deletes the file.
        /// </summary>
        public bool SaveNullDeletesFile { get; }
    }
}
