namespace Gu.Settings
{
    using System;
    using System.IO;

    public class RepositorySetting
    {
        public RepositorySetting(
            bool createBackupOnSave,
            DirectoryInfo directory,
            string extension = ".cfg",
            string backupExtension = ".old")
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException("Extension cannot be empty", "extension");
            }
            if (CreateBackupOnSave && string.IsNullOrEmpty(backupExtension))
            {
                throw new ArgumentException("Extension cannot be empty", "backupExtension");
            }
            if (directory == null)
            {
                throw new ArgumentNullException("directory");
            }
            CreateBackupOnSave = createBackupOnSave;
            Directory = directory;
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            Extension = extension;
            if (CreateBackupOnSave && !backupExtension.StartsWith("."))
            {
                backupExtension = "." + backupExtension;
            }
            BackupExtension = backupExtension;
        }

        public bool CreateBackupOnSave { get; private set; }

        public DirectoryInfo Directory { get; private set; }

        public string Extension { get; private set; }

        public string BackupExtension { get; private set; }
    }
}
