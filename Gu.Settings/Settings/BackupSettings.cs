﻿namespace Gu.Settings
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    [Serializable]
    public class BackupSettings : IFileSettings
    {
        public static BackupSettings None = new BackupSettings(false, null, null, null, false, 1, 1);
        public static readonly string DefaultTimeStampFormat = ".yyyy_MM_dd_HH_mm_ss";
        private string _directoryPath;
        private string _extension;
        private bool _createBackups;
        private string _timeStampFormat;
        private bool _hidden;
        private int _numberOfBackups;
        private int _maxAgeInDays;

        private BackupSettings() // needed for XmlSerializer
        {
        }

        public BackupSettings(
            bool createBackups,
            DirectoryInfo directory,
            string extension,
            string timeStampFormat,
            bool hidden,
            int numberOfBackups,
            int maxAgeInDays)
        {
            if (createBackups)
            {
                Ensure.NotNull(directory, "directory");
                Ensure.NotNullOrEmpty(extension, "extension");
                DirectoryPath = directory.FullName;
            }
            CreateBackups = createBackups;

            if (!string.IsNullOrWhiteSpace(timeStampFormat))
            {
                TimeStampFormat = timeStampFormat;
            }
            if (numberOfBackups > 1)
            {
                TimeStampFormat = DefaultTimeStampFormat;
            }
            Extension = FileHelper.PrependDotIfMissing(extension);
            Hidden = hidden;
            NumberOfBackups = numberOfBackups;
            MaxAgeInDays = maxAgeInDays;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DirectoryInfo Directory
        {
            get { return new DirectoryInfo(DirectoryPath); }
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

        public bool CreateBackups
        {
            get { return _createBackups; }
            set
            {
                if (value == _createBackups)
                {
                    return;
                }
                _createBackups = value;
                OnPropertyChanged();
            }
        }

        public string TimeStampFormat
        {
            get { return _timeStampFormat; }
            set
            {
                value = FileHelper.PrependDotIfMissing(value); // this is not very nice, hack for now
                if (value == _timeStampFormat)
                {
                    return;
                }
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
            return new BackupSettings(true, directory, ".bak", null, false, 1, int.MaxValue);
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