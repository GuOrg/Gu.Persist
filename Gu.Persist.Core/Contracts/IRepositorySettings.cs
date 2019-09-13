namespace Gu.Persist.Core
{
    /// <summary>
    /// Settings for <see cref="Repository{TSetting}"/>.
    /// </summary>
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
        /// Gets a value indicating whether the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes.
        /// </summary>
        bool IsTrackingDirty { get; }
    }
}