namespace Gu.Settings
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    [Serializable]
    public class BackupSettings : IBackupSettings
    {
        public static readonly string DefaultTimeStampFormat = "yyyy_MM_dd_HH_mm_ss";
        public static readonly string DefaultExtension = ".bak";
        private string _directoryPath;
        private string _extension;
        private bool _isCreatingBackups;
        private string _timeStampFormat;
        private bool _hidden;
        private int _numberOfBackups;
        private int _maxAgeInDays;

        protected BackupSettings() // needed for XmlSerializer
        {
        }

        public BackupSettings(DirectoryInfo directory)
            : this(directory, true, DefaultExtension, null, false, 1, int.MaxValue)
        {
        }

        public BackupSettings(DirectoryInfo directory, int numberOfBackups)
            : this(directory, true, DefaultExtension, DefaultTimeStampFormat, false, numberOfBackups, int.MaxValue)
        {
        }

        public BackupSettings(DirectoryInfo directory, int numberOfBackups, int maxAgeInDays)
            : this(directory, true, DefaultExtension, DefaultTimeStampFormat, false, numberOfBackups, maxAgeInDays)
        {
        }

        public BackupSettings(
            DirectoryInfo directory,
            bool isCreatingBackups,
            string extension,
            string timeStampFormat,
            bool hidden,
            int numberOfBackups,
            int maxAgeInDays)
        {
            if (isCreatingBackups)
            {
                Ensure.NotNull(directory, nameof(directory));
                Ensure.NotNullOrEmpty(extension, nameof(extension));
                ValidateTimestampFormat(timeStampFormat);
                _directoryPath = directory.FullName;
            }
            _isCreatingBackups = isCreatingBackups;

            if (!string.IsNullOrWhiteSpace(timeStampFormat))
            {
                _timeStampFormat = timeStampFormat;
            }
            if (numberOfBackups > 1)
            {
                _timeStampFormat = DefaultTimeStampFormat;
            }
            _extension = FileHelper.PrependDotIfMissing(extension);
            _hidden = hidden;
            _numberOfBackups = numberOfBackups;
            _maxAgeInDays = maxAgeInDays;
        }

        [field: NonSerialized]
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

        public bool IsCreatingBackups
        {
            get { return _isCreatingBackups; }
            set
            {
                if (value == _isCreatingBackups)
                {
                    return;
                }
                _isCreatingBackups = value;
                OnPropertyChanged();
            }
        }

        public string TimeStampFormat
        {
            get { return _timeStampFormat; }
            set
            {
                if (value == _timeStampFormat)
                {
                    return;
                }
                ValidateTimestampFormat(value);
                _timeStampFormat = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets if backup files shall be hidden
        /// </summary>
        public bool Hidden
        {
            get { return _hidden; }
            set
            {
                if (value == _hidden)
                {
                    return;
                }
                _hidden = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gest the number of backups to keep.
        /// </summary>
        public int NumberOfBackups
        {
            get { return _numberOfBackups; }
            set
            {
                if (value == _numberOfBackups)
                {
                    return;
                }
                _numberOfBackups = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the max age of backups.
        /// </summary>
        public int MaxAgeInDays
        {
            get { return _maxAgeInDays; }
            set
            {
                if (value == _maxAgeInDays)
                {
                    return;
                }
                _maxAgeInDays = value;
                OnPropertyChanged();
            }
        }

        public static BackupSettings DefaultFor(DirectoryInfo directory)
        {
            return new BackupSettings(directory, true, BackupSettings.DefaultExtension, null, false, 1, int.MaxValue);
        }

        public static void ValidateTimestampFormat(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var time = new DateTime(2015, 06, 14, 11, 54, 25);
                var s = time.ToString(value, CultureInfo.InvariantCulture);
                var roundtrip = DateTime.ParseExact(s, value, CultureInfo.InvariantCulture);
                if (time != roundtrip)
                {
                    throw new ArgumentException(
                        string.Format("The format: {0} is not valid as it cannot be roundtripped", value),
                        "value");
                }
            }
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
