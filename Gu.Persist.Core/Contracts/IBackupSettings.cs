namespace Gu.Persist.Core
{
    public interface IBackupSettings : IFileSettings
    {
        /// <summary>
        /// Gest if backups should be created.
        /// </summary>
        bool IsCreatingBackups { get; }

        /// <summary>
        /// Gest the format for timestamps
        /// </summary>
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