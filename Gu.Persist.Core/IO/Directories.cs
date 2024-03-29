﻿// ReSharper disable UnusedMember.Global
namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    /// <summary>
    /// Exposes common system directories.
    /// </summary>
    public static class Directories
    {
        private static DirectoryInfo? @default;
        private static DirectoryInfo? defaultBackup;

        /// <summary>
        /// Gets the <see cref="Environment.CurrentDirectory"/>.
        /// </summary>
        public static DirectoryInfo CurrentDirectory => new(Environment.CurrentDirectory);

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).
        /// </summary>
        public static DirectoryInfo ApplicationData => new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).
        /// </summary>
        public static DirectoryInfo LocalApplicationData => new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).
        /// </summary>
        public static DirectoryInfo CommonApplicationData => new(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).
        /// </summary>
        public static DirectoryInfo MyDocuments => new(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments).
        /// </summary>
        public static DirectoryInfo CommonDocuments => new(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).
        /// </summary>
        public static DirectoryInfo ProgramFiles => new(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).
        /// </summary>
        public static DirectoryInfo ProgramFilesX86 => new(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles).
        /// </summary>
        public static DirectoryInfo CommonProgramFiles => new(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86).
        /// </summary>
        public static DirectoryInfo CommonProgramFilesX86 => new(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.Desktop).
        /// </summary>
        public static DirectoryInfo Desktop => new(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

        /// <summary>
        /// Gets Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory).
        /// </summary>
        public static DirectoryInfo DesktopDirectory => new(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

        /// <summary>
        /// Gets Path.GetTempPath().
        /// </summary>
        public static DirectoryInfo TempDirectory => new(Path.GetTempPath());

        /// <summary>
        /// Gets %AppData%\ApplicationName.
        /// </summary>
        public static DirectoryInfo Default => @default ??= ApplicationData.CreateSubdirectory(AppDirectory().Name);

        /// <summary>
        /// Gets %AppData%\ApplicationName\Backup.
        /// </summary>
        public static DirectoryInfo DefaultBackup => defaultBackup ??= Default.CreateSubdirectory("Backup");

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

            return directory!;
        }
    }
}
