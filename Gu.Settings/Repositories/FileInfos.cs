namespace Gu.Settings.Repositories
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
            var backup = CreateFileInfo(directory, fileName, backupExtension);
            return new FileInfos(fileInfo, backup);
        }

        private FileInfo CreateFileInfo(DirectoryInfo directory, string fileName, string extension)
        {
            throw new NotImplementedException("Check and append extension if needed");
            var fullFileName = System.IO.Path.Combine(directory.FullName, fileName);
            return new FileInfo(fullFileName);
        }
    }
}