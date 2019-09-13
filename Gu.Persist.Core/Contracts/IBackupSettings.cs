namespace Gu.Persist.Core
{
    public interface IBackupSettings : IFileSettings
    {
        /// <summary>
        /// Gets the format for timestamps used as suffix on backup files.
        /// This must be a valid format string for <see cref="System.DateTime"/>
        /// Default is yyyy_MM_dd_HH_mm_ss.
        /// </summary>
        string TimeStampFormat { get; }

        /////// <summary>
        /////// Gets if backup files shall be hidden
        /////// </summary>
        ////bool Hidden { get; }

        /// <summary>
        /// Gets the number of backups to keep.
        /// If 0 an unlimited amount of backups are kept.
        /// </summary>
        int NumberOfBackups { get; }

        /// <summary>
        /// Gets the max age of backups.
        /// If 0 unlimited age is allowed.
        /// </summary>
        int MaxAgeInDays { get; }
    }
}