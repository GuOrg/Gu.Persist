namespace Gu.Settings.Core
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class Directories
    {
        public static readonly DirectoryInfo CurrentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

        public static readonly DirectoryInfo ApplicationData = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        public static readonly DirectoryInfo LocalApplicationData = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        public static readonly DirectoryInfo CommonApplicationData = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
        public static readonly string AppDataVariable = "%AppData%";

        public static readonly DirectoryInfo MyDocuments = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        public static readonly DirectoryInfo CommonDocuments = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));

        public static readonly DirectoryInfo ProgramFiles = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        public static readonly DirectoryInfo ProgramFilesX86 = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

        public static readonly DirectoryInfo CommonProgramFiles = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
        public static readonly DirectoryInfo CommonProgramFilesX86 = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));

        public static readonly DirectoryInfo Desktop = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        public static readonly DirectoryInfo DesktopDirectory = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        public static readonly DirectoryInfo TempDirectory = CreateInfo(Path.GetTempPath());

        private static DirectoryInfo _default;
        public static DirectoryInfo Default
        {
            get
            {
                if (_default == null)
                {
                    _default = ApplicationData.Subdirectory(Assembly.GetExecutingAssembly().GetName().Name)
                                              .Subdirectory("Settings");
                }
                return _default;
            }
            set { _default = value; }
        }

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

        internal static string GetPath(DirectoryInfo info)
        {
            var path = TryGetRelativePath(info, CurrentDirectory, ".") ??
                       TryGetRelativePath(info, ApplicationData, AppDataVariable) ??
                       info.FullName;
            return path;
        }

        private static string TryGetRelativePath(DirectoryInfo info, DirectoryInfo directory, string prefix)
        {
            if (directory == null)
            {
                return null;
            }
            if (info.IsStrictSubDirectoryOf(directory))
            {
                var relativePath = info.FullName.Substring(directory.FullName.Length);
                return $"{prefix}{relativePath}";
            }
            return null;
        }
    }
}
