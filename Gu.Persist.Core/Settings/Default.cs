namespace Gu.Persist.Core
{
    using System.IO;

    public static class Default
    {
        public static BackupSettings BackupSettings(DirectoryInfo directory)
        {
            return new BackupSettings(
                       directory.FullName,
                       Core.BackupSettings.DefaultExtension,
                       null,
                       1,
                       int.MaxValue);
        }

        public static RepositorySettings RepositorySettings(DirectoryInfo directory)
        {
            return new RepositorySettings(directory.FullName, false, BackupSettings(directory));
        }

        public static DataRepositorySettings DataRepositorySettings(DirectoryInfo directory)
        {
            return new DataRepositorySettings(
                       directory.FullName,
                       false,
                       false,
                       BackupSettings(directory));
        }
    }
}
