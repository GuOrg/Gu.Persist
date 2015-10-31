namespace Gu.Settings.Core
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class Directories
    {
        public static DirectoryInfo CurrentDirectory => new DirectoryInfo(Environment.CurrentDirectory);

        public static DirectoryInfo ApplicationData => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        public static DirectoryInfo LocalApplicationData => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        public static DirectoryInfo CommonApplicationData => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

        public static DirectoryInfo MyDocuments => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        public static DirectoryInfo CommonDocuments => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));

        public static DirectoryInfo ProgramFiles => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        public static DirectoryInfo ProgramFilesX86 => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

        public static DirectoryInfo CommonProgramFiles => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
        public static DirectoryInfo CommonProgramFilesX86 => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));

        public static DirectoryInfo Desktop => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        public static DirectoryInfo DesktopDirectory => CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        public static DirectoryInfo TempDirectory => CreateInfo(Path.GetTempPath());

        private static DirectoryInfo _default;
        public static DirectoryInfo Default => _default ?? (_default = PathAndSpecialFolder.Default.CreateDirectoryInfo());

        internal static DirectoryInfo CreateInfo(string path)
        {
            if (path.StartsWith("%"))
            {
                path = Environment.ExpandEnvironmentVariables(path);
            }
            if (path.StartsWith("."))
            {
                path = Path.Combine(Environment.CurrentDirectory, path);
            }
            return new DirectoryInfo(path);
        }
    }
}
