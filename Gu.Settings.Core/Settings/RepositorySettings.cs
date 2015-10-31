namespace Gu.Settings.Core
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Core.Properties;

    [Serializable]
    public class RepositorySettings : IRepositorySettings
    {
        protected static readonly string BackupDirectoryName = "Backup";
        private string _directoryPath;
        private BackupSettings _backupSettings;
        private string _extension;
        private string _tempExtension;
        private bool _isTrackingDirty;
        private bool _isCaching;
        private DirectoryInfo _directory;

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
        {
            Ensure.NotNullOrEmpty(extension, nameof(extension));
            Ensure.NotNullOrEmpty(tempExtension, nameof(tempExtension));
            Ensure.NotNull(directory, nameof(directory));

            _isTrackingDirty = isTrackingDirty;
            _isCaching = isCaching;
            _directoryPath = directory.FullName;
            _backupSettings = backupSettings;

            _extension = FileHelper.PrependDotIfMissing(extension);
            _tempExtension = FileHelper.PrependDotIfMissing(tempExtension);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public DirectoryInfo Directory
        {
            get { return _directory ?? (_directory = Directories.CreateInfo(_directoryPath)); }
            private set
            {
                if (Equals(value, _directory)) return;
                _directory = value;
                OnPropertyChanged();
            }
        }

        public string DirectoryPath
        {
            get { return _directoryPath; }
            set
            {
                if (value == _directoryPath)
                {
                    return;
                }
                _directoryPath = value;
                Directory = Directories.CreateInfo(_directoryPath);
                OnPropertyChanged();
            }
        }

        public string Extension
        {
            get { return _extension; }
            set
            {
                if (value == _extension)
                {
                    return;
                }
                _extension = value;
                OnPropertyChanged();
            }
        }

        public BackupSettings BackupSettings
        {
            get { return _backupSettings; }
            set
            {
                if (Equals(value, _backupSettings))
                {
                    return;
                }
                _backupSettings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the file extension used when saving files.
        /// On successful save the file extension is replaced.
        /// </summary>
        public string TempExtension
        {
            get { return _tempExtension; }
            set
            {
                if (value == _tempExtension)
                {
                    return;
                }
                _tempExtension = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes
        /// </summary>
        public bool IsTrackingDirty
        {
            get { return _isTrackingDirty; }
            set
            {
                if (value == _isTrackingDirty)
                {
                    return;
                }
                _isTrackingDirty = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if the repository keeps a cache of instances saved/read. Default is true, setting to false gives new instance for each read.
        /// </summary>
        public bool IsCaching
        {
            get { return _isCaching; }
            set
            {
                if (value == _isCaching)
                {
                    return;
                }
                _isCaching = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Creates a <see cref="RepositorySettings"/> for <paramref name="directory"/>
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static RepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new RepositorySettings(directory, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(BackupDirectoryName)));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
