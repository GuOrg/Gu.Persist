namespace Gu.Settings.Core
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class Directories
    {
        public static readonly DirectoryInfo ExecutingDirectory = CreateInfo(Assembly.GetExecutingAssembly().Location);

        public static readonly DirectoryInfo ApplicationData = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        public static readonly DirectoryInfo LocalApplicationData = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        public static readonly DirectoryInfo CommonApplicationData = CreateInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

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

        private static DirectoryInfo CreateInfo(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            return new DirectoryInfo(directoryName);
        }
    }
}
