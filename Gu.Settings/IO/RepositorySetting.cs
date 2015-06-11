namespace Gu.Settings
{
    using System;
    using System.IO;

    public class RepositorySetting : IRepositorySetting
    {
        public RepositorySetting(
            bool createBackupOnSave,
            bool isTrackingDirty,
            DirectoryInfo directory,
            string extension = ".cfg",
            string tempExtension = ".tmp",
            string backupExtension = ".old")
        {
            Ensure.NotNullOrEmpty(extension, "extension");
            Ensure.NotNullOrEmpty(tempExtension, "tempExtension");
            Ensure.NotNull(directory, "directory");
            if (CreateBackupOnSave)
            {
                Ensure.NotNullOrEmpty(backupExtension, "backupExtension");
            }

            CreateBackupOnSave = createBackupOnSave;
            IsTrackingDirty = isTrackingDirty;
            Directory = directory;

            Extension = PrependDotToExtension(extension);
            TempExtension = PrependDotToExtension(tempExtension);
            BackupExtension = PrependDotToExtension(backupExtension);
        }

        public bool CreateBackupOnSave { get; private set; }

        public DirectoryInfo Directory { get; private set; }

        public string Extension { get; private set; }

        public string TempExtension { get; private set; }

        public string BackupExtension { get; private set; }

        public bool IsTrackingDirty { get; private set; }

        private string PrependDotToExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return extension;
            }
            if (!extension.StartsWith("."))
            {
                return "." + extension;
            }
            return extension;
        }
    }
}
