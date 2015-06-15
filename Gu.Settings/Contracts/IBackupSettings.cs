namespace Gu.Settings
{
    public interface IBackupSettings : IFileSettings
    {
        string DirectoryPath { get; }
        bool CreateBackups { get; }
        string TimeStampFormat { get; }
        /// <summary>
        /// Gets if backup files shall be hidden
        /// </summary>
        bool Hidden { get; }
        /// <summary>
        /// Gest the number of backups to keep.
        /// </summary>
        int NumberOfBackups { get; }
        /// <summary>
        /// Gets the max age of backups.
        /// </summary>
        int MaxAgeInDays { get; }
    }
}