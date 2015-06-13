namespace Gu.Settings
{
    using System;
    using System.IO;

    [Serializable]
    internal class FileInfos : IFileInfos
    {
        public FileInfos(FileInfo file, FileInfo tempFile, FileInfo backup)
        {
            Ensure.NotNull(file, "file");
            File = file;
            TempFile = tempFile;
            Backup = backup;
        }

        public FileInfo File { get; private set; }

        public FileInfo TempFile { get; private set; }

        public FileInfo Backup { get; private set; }

        public static FileInfos CreateFileInfos(
            DirectoryInfo directory,
            string fileName,
            string extension,
            string tempExtension,
            string backupExtension)
        {
            Ensure.NotNull(directory, "directory");
            Ensure.NotNullOrEmpty(fileName, "fileName");
            Ensure.NotNullOrEmpty(extension, "extension");
            Ensure.NotNullOrEmpty(backupExtension, "backupExtension");
            var fileInfo = FileHelper.CreateFileInfo(directory, fileName, extension);
            return CreateFileInfos(fileInfo, tempExtension, backupExtension);
        }

        public static FileInfos CreateFileInfos(FileInfo fileInfo, string tempExtension, string backupExtension)
        {
            Ensure.NotNull(fileInfo, "fileInfo");
            Ensure.NotNullOrEmpty(tempExtension, "tempExtension");
            var temp = FileHelper.ChangeExtension(fileInfo, tempExtension);
            var backup = string.IsNullOrEmpty(backupExtension)
                             ? null
                             : FileHelper.ChangeExtension(fileInfo, backupExtension);
            return new FileInfos(fileInfo, temp, backup);
        }
    }
}