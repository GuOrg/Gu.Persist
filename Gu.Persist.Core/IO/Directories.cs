// ReSharper disable UnusedMember.Global
namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    public static class Directories
    {
        private static DirectoryInfo @default;
        private static DirectoryInfo defaultBackup;

        /// <summary>
        /// <see cref="Environment.CurrentDirectory"/>.
        /// </summary>
        public static DirectoryInfo CurrentDirectory => new DirectoryInfo(Environment.CurrentDirectory);

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).
        /// </summary>
        public static DirectoryInfo ApplicationData => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).
        /// </summary>
        public static DirectoryInfo LocalApplicationData => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).
        /// </summary>
        public static DirectoryInfo CommonApplicationData => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).
        /// </summary>
        public static DirectoryInfo MyDocuments => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments).
        /// </summary>
        public static DirectoryInfo CommonDocuments => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).
        /// </summary>
        public static DirectoryInfo ProgramFiles => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).
        /// </summary>
        public static DirectoryInfo ProgramFilesX86 => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles).
        /// </summary>
        public static DirectoryInfo CommonProgramFiles => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86).
        /// </summary>
        public static DirectoryInfo CommonProgramFilesX86 => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.Desktop).
        /// </summary>
        public static DirectoryInfo Desktop => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory).
        /// </summary>
        public static DirectoryInfo DesktopDirectory => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

        /// <summary>
        /// Path.GetTempPath().
        /// </summary>
        public static DirectoryInfo TempDirectory => new DirectoryInfo(Path.GetTempPath());

        /// <summary>
        /// %AppData%\ApplicationName.
        /// </summary>
        public static DirectoryInfo Default => @default ?? (@default = ApplicationData.CreateSubdirectory(AppDirectory().Name));

        /// <summary>
        /// %AppData%\ApplicationName\Backup.
        /// </summary>
        public static DirectoryInfo DefaultBackup => defaultBackup ?? (defaultBackup = Default.CreateSubdirectory("Backup"));

        internal static DirectoryInfo AppDirectory()
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory);
            while (directory != null &&
                   (string.Equals("bin", directory.Name, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals("debug", directory.Name, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals("release", directory.Name, StringComparison.OrdinalIgnoreCase)))
            {
                directory = directory.Parent;
            }

            return directory;
        }
    }
}
