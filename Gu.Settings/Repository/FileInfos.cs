namespace Gu.Settings
{
    using System;
    using System.IO;

    internal class FileInfos
    {
        private FileInfos(FileInfo file, FileInfo backup)
        {
            File = file;
            Backup = backup;
        }

        public FileInfo File { get; private set; }

        public FileInfo Backup { get; private set; }

        public object Value { get; internal set; }

        internal static FileInfos CreateFileInfos(DirectoryInfo directory, string fileName, bool hasBackup, string extension, string backupExtension)
        {
            var fileInfo = CreateFileInfo(directory, fileName, extension);
            if (!hasBackup)
            {
                return new FileInfos(fileInfo, null);
            }
            var backup = BackupFile(fileInfo, backupExtension);
            return new FileInfos(fileInfo, backup);
        }

        private static FileInfo CreateFileInfo(DirectoryInfo directory, string fileName, string extension)
        {
            if (!fileName.EndsWith(extension))
            {
                fileName += extension;
            }
            var fullFileName = System.IO.Path.Combine(directory.FullName, fileName);
            return new FileInfo(fullFileName);
        }

        internal static FileInfo BackupFile(FileInfo file, string backupExtension = ".old")
        {
            var backupFile = new FileInfo(file.FullName.Replace(file.Extension, backupExtension));
            return backupFile;
        }
    }
}