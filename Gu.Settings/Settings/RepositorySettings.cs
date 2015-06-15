namespace Gu.Settings
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    [Serializable]
    public class RepositorySettings : IRepositorySettings
    {
        private string _directoryPath;
        private BackupSettings _backupSettings;
        private string _extension;
        private string _tempExtension;
        private bool _isTrackingDirty;

        private RepositorySettings() // needed for XmlSerializer
        {
        }

        public RepositorySettings(DirectoryInfo directory)
            : this(directory, Gu.Settings.BackupSettings.DefaultFor(directory))
        {
        }

        public RepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : this(true, directory, backupSettings)
        {
        }

        public RepositorySettings(
            bool isTrackingDirty,
            DirectoryInfo directory,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp"
            )
        {
            Ensure.NotNullOrEmpty(extension, "extension");
            Ensure.NotNullOrEmpty(tempExtension, "tempExtension");
            Ensure.NotNull(directory, "directory");

            _isTrackingDirty = isTrackingDirty;
            _directoryPath = directory.FullName;
            _backupSettings = backupSettings;

            _extension = FileHelper.PrependDotIfMissing(extension);
            _tempExtension = FileHelper.PrependDotIfMissing(tempExtension);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DirectoryInfo Directory
        {
            get
            {
                if (string.IsNullOrEmpty(DirectoryPath))
                {
                    return null;
                }
                return new DirectoryInfo(DirectoryPath);
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
                OnPropertyChanged();
                OnPropertyChanged("Directory");
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

        public static RepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new RepositorySettings(true, directory, Settings.BackupSettings.DefaultFor(directory));
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
