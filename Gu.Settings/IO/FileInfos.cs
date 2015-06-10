namespace Gu.Settings
{
    using System;
    using System.IO;

    [Serializable]
    public class FileInfos : IFileInfos
    {
        public FileInfos(FileInfo file, FileInfo backup)
        {
            Ensure.NotNull(file, "file");
            File = file;
            Backup = backup;
        }

        public FileInfo File { get; private set; }

        public FileInfo Backup { get; private set; }

        public static FileInfos CreateFileInfos(DirectoryInfo directory, string fileName, string extension)
        {
            Ensure.NotNull(directory, "directory");
            Ensure.NotNullOrEmpty(fileName, "fileName");
            Ensure.NotNullOrEmpty(extension, "extension");
            var fileInfo = FileHelper.CreateFileInfo(directory, fileName, extension);
            return CreateFileInfos(fileInfo);
        }

        public static FileInfos CreateFileInfos(DirectoryInfo directory, string fileName, string extension, string backupExtension)
        {
            Ensure.NotNull(directory, "directory");
            Ensure.NotNullOrEmpty(fileName, "fileName");
            Ensure.NotNullOrEmpty(extension, "extension");
            Ensure.NotNullOrEmpty(backupExtension, "backupExtension");
            var fileInfo = FileHelper.CreateFileInfo(directory, fileName, extension);
            return CreateFileInfos(fileInfo, backupExtension);
        }

        public static FileInfos CreateFileInfos(FileInfo fileInfo)
        {
            Ensure.NotNull(fileInfo, "fileInfo");
            return new FileInfos(fileInfo, null);
        }

        public static FileInfos CreateFileInfos(FileInfo fileInfo, string backupExtension)
        {
            Ensure.NotNull(fileInfo, "fileInfo");
            Ensure.NotNull(backupExtension, "backupExtension");
            var backup = BackupFile(fileInfo, backupExtension);
            return new FileInfos(fileInfo, backup);
        }

        internal static FileInfo BackupFile(FileInfo file, string backupExtension = ".old")
        {
            var changeExtension = Path.ChangeExtension(file.FullName, backupExtension);
            var backupFile = new FileInfo(changeExtension);
            return backupFile;
        }
    }
}