namespace Gu.Settings.Repositories
{
    using System.IO;

    public class RepositorySetting
    {
        public RepositorySetting(
            bool createBackupOnSave,
            DirectoryInfo directory,
            string extension = ".cfg",
            string backupExtension = ".old")
        {
            CreateBackupOnSave = createBackupOnSave;
            Directory = directory;
            Extension = extension;
            BackupExtension = backupExtension;
        }

        public bool CreateBackupOnSave { get; private set; }

        public DirectoryInfo Directory { get; private set; }

        public string Extension { get; private set; }

        public string BackupExtension { get; private set; }
    }
}
