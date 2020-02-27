namespace Gu.Persist.Core
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Specifies the behavior of an <see cref="IBackuper"/>.
    /// </summary>
    [Serializable]
    public class BackupSettings : FileSettings, IBackupSettings
    {
        /// <summary>
        /// The default timestamp format.
        /// </summary>
        public static readonly string DefaultTimeStampFormat = "yyyy_MM_dd_HH_mm_ss";

        /// <summary>
        /// The default backup file extension.
        /// </summary>
        public static readonly string DefaultExtension = ".bak";

        /// <summary>Initializes a new instance of the <see cref="BackupSettings"/> class.</summary>
        /// <param name="directory">The directory path.</param>
        /// <param name="extension">The backup file extension.</param>
        /// <param name="timeStampFormat">The timestamp suffix format. Example 'yyyy_MM_dd_HH_mm_ss'.</param>
        /// <param name="numberOfBackups">The max number of backups tro keep.</param>
        /// <param name="maxAgeInDays">The max age of backups.</param>
        public BackupSettings(
            string directory,
            string extension,
            string? timeStampFormat,
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

        /// <summary>
        /// Gets the format of the timestamp suffix. Example 'yyyy_MM_dd_HH_mm_ss'.
        /// </summary>
        public string? TimeStampFormat { get; }

        /// <summary>
        /// Gets the number of backups to keep.
        /// </summary>
        public int NumberOfBackups { get; }

        /// <summary>
        /// Gets the max age of backups.
        /// </summary>
        public int MaxAgeInDays { get; }

        /// <summary>
        /// Check if <paramref name="format"/> is a valid timestamp format.
        /// Valid formats can be roundtripped using <see cref="DateTime.ToString(string)"/> and <see cref="DateTime.Parse(string)"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        public static void ValidateTimestampFormat(string? format)
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
