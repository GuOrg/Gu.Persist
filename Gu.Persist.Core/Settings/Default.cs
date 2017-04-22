namespace Gu.Persist.Core
{
    using System.IO;

    public static class Default
    {
        public static BackupSettings BackupSettings(DirectoryInfo directory)
        {
            return new BackupSettings(
                       directory: directory.FullName,
                       extension: Core.BackupSettings.DefaultExtension,
                       timeStampFormat: null,
                       numberOfBackups: 1,
                       maxAgeInDays: int.MaxValue);
        }

        public static RepositorySettings RepositorySettings(DirectoryInfo directory)
        {
            return new RepositorySettings(
                directory: directory.FullName,
                isTrackingDirty: false,
                backupSettings: BackupSettings(directory));
        }

        public static DataRepositorySettings DataRepositorySettings(DirectoryInfo directory)
        {
            return new DataRepositorySettings(
                       directory: directory.FullName,
                       isTrackingDirty: false,
                       saveNullDeletesFile: false,
                       backupSettings: BackupSettings(directory));
        }
    }
}
