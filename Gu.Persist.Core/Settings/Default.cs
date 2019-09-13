namespace Gu.Persist.Core
{
    using System.IO;

#pragma warning disable CA1716 // Identifiers should not match keywords
    /// <summary>
    /// Contains factory methods for creating default settings.
    /// </summary>
    public static class Default
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        /// <summary>
        /// Create default <see cref="BackupSettings"/>.
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo"/>.</param>
        /// <returns>A <see cref="BackupSettings"/>.</returns>
        public static BackupSettings BackupSettings(DirectoryInfo directory)
        {
            if (directory is null)
            {
                throw new System.ArgumentNullException(nameof(directory));
            }

            return new BackupSettings(
                       directory: directory.FullName,
                       extension: Core.BackupSettings.DefaultExtension,
                       timeStampFormat: null,
                       numberOfBackups: 1,
                       maxAgeInDays: int.MaxValue);
        }

        /// <summary>
        /// Create default <see cref="RepositorySettings"/>.
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo"/>.</param>
        /// <returns>A <see cref="RepositorySettings"/>.</returns>
        public static RepositorySettings RepositorySettings(DirectoryInfo directory)
        {
            if (directory is null)
            {
                throw new System.ArgumentNullException(nameof(directory));
            }

            return new RepositorySettings(
                directory: directory.FullName,
                isTrackingDirty: false,
                backupSettings: BackupSettings(directory));
        }

        /// <summary>
        /// Create default <see cref="DataRepositorySettings"/>.
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo"/>.</param>
        /// <returns>A <see cref="DataRepositorySettings"/>.</returns>
        public static DataRepositorySettings DataRepositorySettings(DirectoryInfo directory)
        {
            if (directory is null)
            {
                throw new System.ArgumentNullException(nameof(directory));
            }

            return new DataRepositorySettings(
                       directory: directory.FullName,
                       isTrackingDirty: false,
                       saveNullDeletesFile: false,
                       backupSettings: BackupSettings(directory));
        }
    }
}
