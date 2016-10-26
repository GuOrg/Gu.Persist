namespace Gu.Settings.Core
{
    using System;
    using System.IO;

    [Serializable]
    public class RepositorySettings : FileSettings, IRepositorySettings
    {
        protected static readonly string DefaultBackupDirectoryName = "Backup";
        private BackupSettings backupSettings;
        private string tempExtension;
        private bool isTrackingDirty;
        private bool isCaching;

        protected RepositorySettings() // needed for XmlSerializer
        {
        }

        public RepositorySettings(DirectoryInfo directory)
            : this(directory, BackupSettings.DefaultFor(directory))
        {
        }

        public RepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : this(directory, true, true, backupSettings)
        {
        }

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

            this.isTrackingDirty = isTrackingDirty;
            this.isCaching = isCaching;
            this.backupSettings = backupSettings;
            this.tempExtension = FileHelper.PrependDotIfMissing(tempExtension);
        }

        public BackupSettings BackupSettings
        {
            get
            {
                return this.backupSettings;
            }

            set
            {
                if (Equals(value, this.backupSettings))
                {
                    return;
                }

                this.backupSettings = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the file extension used when saving files.
        /// On successful save the file extension is replaced.
        /// </summary>
        public string TempExtension
        {
            get
            {
                return this.tempExtension;
            }

            set
            {
                if (value == this.tempExtension)
                {
                    return;
                }

                this.tempExtension = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes
        /// </summary>
        public bool IsTrackingDirty
        {
            get
            {
                return this.isTrackingDirty;
            }

            set
            {
                if (value == this.isTrackingDirty)
                {
                    return;
                }

                this.isTrackingDirty = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if the repository keeps a cache of instances saved/read. Default is true, setting to false gives new instance for each read.
        /// </summary>
        public bool IsCaching
        {
            get
            {
                return this.isCaching;
            }

            set
            {
                if (value == this.isCaching)
                {
                    return;
                }

                this.isCaching = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Creates a <see cref="RepositorySettings"/> for <paramref name="directory"/>
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static RepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new RepositorySettings(directory, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}
