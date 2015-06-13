namespace Gu.Settings.IO
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class Directories
    {
        public static DirectoryInfo ExecutingDirectory = CreateInfo(Assembly.GetExecutingAssembly().Location);
        
        public static DirectoryInfo ApplicationData = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        public static DirectoryInfo LocalApplicationData = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        public static DirectoryInfo CommonApplicationData = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
       
        public static DirectoryInfo MyDocuments = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        public static DirectoryInfo CommonDocuments = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));

        public static DirectoryInfo ProgramFiles = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        public static DirectoryInfo ProgramFilesX86 = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

        public static DirectoryInfo CommonProgramFiles = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
        public static DirectoryInfo CommonProgramFilesX86 = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));

        public static DirectoryInfo Desktop = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        public static DirectoryInfo DesktopDirectory = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

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

        private static DirectoryInfo CreateInfo(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            return new DirectoryInfo(directoryName);
        }


        /// <summary>
        /// Creates the directory if not exists
        /// </summary>
        /// <param name="directory"></param>
        internal static void CreateIfNotExists(this DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

    }
}
