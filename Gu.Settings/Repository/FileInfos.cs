namespace Gu.Settings
{
    using System;
    using System.IO;

    [Serializable]
    public class FileInfos
    {
        private FileInfos(FileInfo file, FileInfo backup)
        {
            File = file;
            Backup = backup;
        }

        public FileInfo File { get; private set; }

        public FileInfo Backup { get; private set; }

        public object Value { get; internal set; }

        public static FileInfos CreateFileInfos(DirectoryInfo directory, string fileName, bool hasBackup, string extension, string backupExtension)
        {
            var fileInfo = CreateFileInfo(directory, fileName, extension);
            if (!hasBackup)
            {
                return new FileInfos(fileInfo, null);
            }
            var backup = BackupFile(fileInfo, backupExtension);
            return new FileInfos(fileInfo, backup);
        }

        public static FileInfo CreateFileInfo(DirectoryInfo directory, string fileName, string extension)
        {
            if (!fileName.EndsWith(extension))
            {
                fileName += extension;
            }
            var fullFileName = Path.Combine(directory.FullName, fileName);
            return new FileInfo(fullFileName);
        }

        internal static FileInfo BackupFile(FileInfo file, string backupExtension = ".old")
        {
            var backupFile = new FileInfo(file.FullName.Replace(file.Extension, backupExtension));
            return backupFile;
        }
    }
}