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
            : this(directory, BackupSettings.DefaultFor(directory))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// </summary>
        public RepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : this(directory, true, true, backupSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// </summary>
        public RepositorySettings(
            DirectoryInfo directory,
            bool isTrackingDirty,
            bool isCaching,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : this(
                PathAndSpecialFolder.Create(directory),
                isTrackingDirty,
                isCaching,
                backupSettings,
                extension,
                tempExtension)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// </summary>
        public RepositorySettings(
            PathAndSpecialFolder directory,
            bool isTrackingDirty,
            bool isCaching,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(directory, extension)
        {
            Ensure.NotNullOrEmpty(extension, nameof(extension));
            Ensure.NotNullOrEmpty(tempExtension, nameof(tempExtension));
            Ensure.NotNull(directory, nameof(directory));

            this.IsTrackingDirty = isTrackingDirty;
            this.IsCaching = isCaching;
            this.BackupSettings = backupSettings;
            this.TempExtension = FileHelper.PrependDotIfMissing(tempExtension);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// Needed for XmlSerializer
        /// </summary>
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
        /// Gets or sets if the repository keeps a cache of instances saved/read. Default is true, setting to false gives new instance for each read.
        /// </summary>
        public bool IsCaching { get; }

        /// <summary>
        /// Creates a <see cref="RepositorySettings"/> for <paramref name="directory"/>
        /// Uses BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)) as backup settings.
        /// </summary>
        public static RepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new RepositorySettings(directory, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}
