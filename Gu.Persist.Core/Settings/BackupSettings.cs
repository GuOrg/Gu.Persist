namespace Gu.Persist.Core
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

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(DirectoryInfo directory)
            : this(directory, DefaultExtension, null, 1, int.MaxValue)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(DirectoryInfo directory, int numberOfBackups)
            : this(directory, DefaultExtension, DefaultTimeStampFormat, numberOfBackups, int.MaxValue)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(DirectoryInfo directory, int numberOfBackups, int maxAgeInDays)
            : this(directory, DefaultExtension, DefaultTimeStampFormat, numberOfBackups, maxAgeInDays)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(
            DirectoryInfo directory,
            string extension,
            string timeStampFormat,
            int numberOfBackups,
            int maxAgeInDays)
            : this(
                directory != null ? PathAndSpecialFolder.Create(directory) : null,
                extension,
                timeStampFormat,
                numberOfBackups,
                maxAgeInDays)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public BackupSettings(
            PathAndSpecialFolder directory,
            string extension,
            string timeStampFormat,
            int numberOfBackups,
            int maxAgeInDays)
            : base(directory, extension)
        {
            ValidateTimestampFormat(timeStampFormat);
            if (!string.IsNullOrWhiteSpace(timeStampFormat))
            {
                this.TimeStampFormat = timeStampFormat;
            }

            if (numberOfBackups > 1)
            {
                this.TimeStampFormat = DefaultTimeStampFormat;
            }

            this.NumberOfBackups = numberOfBackups;
            this.MaxAgeInDays = maxAgeInDays;
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        protected BackupSettings() // needed for XmlSerializer
        {
        }

        public string TimeStampFormat { get; }

        /// <summary>
        /// Gest the number of backups to keep.
        /// </summary>
        public int NumberOfBackups { get; }

        /// <summary>
        /// Gets the max age of backups.
        /// </summary>
        public int MaxAgeInDays { get; }

        public static BackupSettings DefaultFor(DirectoryInfo directory)
        {
            return new BackupSettings(directory, DefaultExtension, null, 1, int.MaxValue);
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
