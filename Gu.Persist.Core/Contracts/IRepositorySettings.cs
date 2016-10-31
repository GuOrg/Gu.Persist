namespace Gu.Persist.Core
{
    public interface IRepositorySettings : IFileSettings
    {
        /// <summary>
        /// Gets the settings specifying how backups are handled.
        /// If null no backups are made.
        /// </summary>
        BackupSettings BackupSettings { get; }

        /// <summary>
        /// Gets the file extension used when saving files.
        /// On successful save the file extension is replaced.
        /// </summary>
        string TempExtension { get; }

        /// <summary>
        /// Gets if the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes.
        /// </summary>
        bool IsTrackingDirty { get; }

        /// <summary>
        /// When true saving null deletes the file. When false an exception is thrown.
        /// </summary>
        bool SaveNullDeletesFile { get; }
    }
}