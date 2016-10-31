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

        public string TimeStampFormat { get; }

        /// <summary>
        /// Gest the number of backups to keep.
        /// </summary>
        public int NumberOfBackups { get; }

        /// <summary>
        /// Gets the max age of backups.
        /// </summary>
        public int MaxAgeInDays { get; }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public static BackupSettings Create(DirectoryInfo directory)
        {
            return new BackupSettings(PathAndSpecialFolder.Create(directory), DefaultExtension, null, 1, int.MaxValue);
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public static BackupSettings Create(DirectoryInfo directory, int numberOfBackups)
        {
            return new BackupSettings(PathAndSpecialFolder.Create(directory), DefaultExtension, DefaultTimeStampFormat, numberOfBackups, int.MaxValue);
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public static BackupSettings Create(DirectoryInfo directory, int numberOfBackups, int maxAgeInDays)
        {
            return new BackupSettings(PathAndSpecialFolder.Create(directory), DefaultExtension, DefaultTimeStampFormat, numberOfBackups, maxAgeInDays);
        }

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        public static BackupSettings Create(DirectoryInfo directory, string extension, string timeStampFormat, int numberOfBackups, int maxAgeInDays)
        {
            return new BackupSettings(PathAndSpecialFolder.Create(directory), extension, timeStampFormat, numberOfBackups, maxAgeInDays);
        }

        public static void ValidateTimestampFormat(string format)
        {
            if (!string.IsNullOrEmpty(format))
            {
                try
                {
                    var time = new DateTime(2015, 06, 14, 11, 54, 25);
                    var text = time.ToString(format, CultureInfo.InvariantCulture);
                    var roundtrip = DateTime.ParseExact(text, format, CultureInfo.InvariantCulture);
                    text = time.ToString(format, CultureInfo.InvariantCulture);
                    var roundtrip2 = DateTime.ParseExact(text, format, CultureInfo.InvariantCulture);
                    if (roundtrip != roundtrip2)
                    {
                        throw new ArgumentException($"The format: {format} is not valid as it cannot be roundtripped");
                    }
                }
                catch (Exception e)
                {
                    var message = $"The timestamp format {format} is not valid.";
                    throw new ArgumentException(message, e);
                }
            }
        }
    }
}
