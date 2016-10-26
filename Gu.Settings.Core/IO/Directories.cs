namespace Gu.Settings.Core
{
    using System;
    using System.IO;

    public static class Directories
    {
        public static DirectoryInfo CurrentDirectory => new DirectoryInfo(Environment.CurrentDirectory);

        public static DirectoryInfo ApplicationData => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        public static DirectoryInfo LocalApplicationData => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

        public static DirectoryInfo CommonApplicationData => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

        public static DirectoryInfo MyDocuments => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        public static DirectoryInfo CommonDocuments => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));

        public static DirectoryInfo ProgramFiles => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

        public static DirectoryInfo ProgramFilesX86 => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

        public static DirectoryInfo CommonProgramFiles => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));

        public static DirectoryInfo CommonProgramFilesX86 => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));

        public static DirectoryInfo Desktop => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

        public static DirectoryInfo DesktopDirectory => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

        public static DirectoryInfo TempDirectory => new DirectoryInfo(Path.GetTempPath());

        private static DirectoryInfo @default;

        public static DirectoryInfo Default => @default ?? (@default = PathAndSpecialFolder.Default.CreateDirectoryInfo());

        private static DirectoryInfo defaultBackup;

        public static DirectoryInfo DefaultBackup => defaultBackup ?? (defaultBackup = PathAndSpecialFolder.DefaultBackup.CreateDirectoryInfo());

        internal static DirectoryInfo AppDirectory()
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory);
            while (directory != null &&
                   (String.Equals("bin", directory.Name, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals("debug", directory.Name, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals("release", directory.Name, StringComparison.OrdinalIgnoreCase)))
            {
                directory = directory.Parent;
            }

            return directory;
        }
    }
}
