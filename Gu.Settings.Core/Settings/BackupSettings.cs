namespace Gu.Settings.Core
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Specifies the behavior of an <see cref="IBackuper"/>
    /// </summary>
    [Serializable]
    public class BackupSettings : FileSettings, IBackupSettings
    {
        public static readonly string DefaultTimeStampFormat = "yyyy_MM_dd_HH_mm_ss";
        public static readonly string DefaultExtension = ".bak";
        private bool isCreatingBackups;
        private string timeStampFormat;
        private bool hidden;
        private int numberOfBackups;
        private int maxAgeInDays;

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(DirectoryInfo directory)
            : this(directory, true, DefaultExtension, null, false, 1, int.MaxValue)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(DirectoryInfo directory, int numberOfBackups)
            : this(directory, true, DefaultExtension, DefaultTimeStampFormat, false, numberOfBackups, int.MaxValue)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(DirectoryInfo directory, int numberOfBackups, int maxAgeInDays)
            : this(directory, true, DefaultExtension, DefaultTimeStampFormat, false, numberOfBackups, maxAgeInDays)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(
            DirectoryInfo directory,
            bool isCreatingBackups,
            string extension,
            string timeStampFormat,
            bool hidden,
            int numberOfBackups,
            int maxAgeInDays)
            : this(
                directory != null ? PathAndSpecialFolder.Create(directory) : null,
                isCreatingBackups,
                extension,
                timeStampFormat,
                hidden,
                numberOfBackups,
                maxAgeInDays)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(
            PathAndSpecialFolder directory,
            bool isCreatingBackups,
            string extension,
            string timeStampFormat,
            bool hidden,
            int numberOfBackups,
            int maxAgeInDays)
            : base(directory, extension)
        {
            if (isCreatingBackups)
            {
                Ensure.NotNull(directory, nameof(directory));
                Ensure.NotNullOrEmpty(extension, nameof(extension));
                ValidateTimestampFormat(timeStampFormat);
            }

            this.isCreatingBackups = isCreatingBackups;

            if (!string.IsNullOrWhiteSpace(timeStampFormat))
            {
                this.timeStampFormat = timeStampFormat;
            }

            if (numberOfBackups > 1)
            {
                this.timeStampFormat = DefaultTimeStampFormat;
            }

            this.hidden = hidden;
            this.numberOfBackups = numberOfBackups;
            this.maxAgeInDays = maxAgeInDays;
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        protected BackupSettings() // needed for XmlSerializer
        {
        }

        public bool IsCreatingBackups
        {
            get
            {
                return this.isCreatingBackups;
            }

            set
            {
                if (value == this.isCreatingBackups)
                {
                    return;
                }

                this.isCreatingBackups = value;
                this.OnPropertyChanged();
            }
        }

        public string TimeStampFormat
        {
            get
            {
                return this.timeStampFormat;
            }

            set
            {
                if (value == this.timeStampFormat)
                {
                    return;
                }

                ValidateTimestampFormat(value);
                this.timeStampFormat = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets if backup files shall be hidden
        /// </summary>
        public bool Hidden
        {
            get
            {
                return this.hidden;
            }

            set
            {
                if (value == this.hidden)
                {
                    return;
                }

                this.hidden = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gest the number of backups to keep.
        /// </summary>
        public int NumberOfBackups
        {
            get
            {
                return this.numberOfBackups;
            }

            set
            {
                if (value == this.numberOfBackups)
                {
                    return;
                }

                this.numberOfBackups = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the max age of backups.
        /// </summary>
        public int MaxAgeInDays
        {
            get
            {
                return this.maxAgeInDays;
            }

            set
            {
                if (value == this.maxAgeInDays)
                {
                    return;
                }

                this.maxAgeInDays = value;
                this.OnPropertyChanged();
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
                    throw new ArgumentException($"The format: {value} is not valid as it cannot be roundtripped");
                }
            }
        }
    }
}
