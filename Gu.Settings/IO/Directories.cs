namespace Gu.Settings.IO
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class Directories
    {
        public static DirectoryInfo ExecutingDirectory = Create(Assembly.GetExecutingAssembly().Location);
        
        public static DirectoryInfo ApplicationData = Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        public static DirectoryInfo LocalApplicationData = Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        public static DirectoryInfo CommonApplicationData = Create(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
       
        public static DirectoryInfo MyDocuments = Create(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        public static DirectoryInfo CommonDocuments = Create(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));

        public static DirectoryInfo ProgramFiles = Create(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        public static DirectoryInfo ProgramFilesX86 = Create(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

        public static DirectoryInfo CommonProgramFiles = Create(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
        public static DirectoryInfo CommonProgramFilesX86 = Create(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));

        public static DirectoryInfo Desktop = Create(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        public static DirectoryInfo DesktopDirectory = Create(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

        public static DirectoryInfo Subdirectory(this DirectoryInfo directory, string name)
        {
            var path = Path.Combine(directory.FullName, name);
            return new DirectoryInfo(path);
        }

        public static FileInfo CreateFileInfoInDirectory(this DirectoryInfo directory, string fileName)
        {
            var path = Path.Combine(directory.FullName, fileName);
            return new FileInfo(path);
        }

        private static DirectoryInfo Create(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            return new DirectoryInfo(directoryName);
        }
    }
}
