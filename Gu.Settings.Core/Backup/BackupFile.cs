namespace Gu.Settings.Core.Backup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gu.Settings.Core.Internals;

    /// <summary>
    ///
    /// </summary>
    public class BackupFile
    {
        private BackupFile(FileInfo file, IBackupSettings setting)
        {
            Ensure.Exists(file);
            Ensure.NotNull(setting, nameof(setting));

            this.File = file;
            this.TimeStamp = file.GetTimeStamp(setting);
        }

        /// <summary>
        /// Gets the <see cref="FileInfo"/>
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> that represents the time of the backup.
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"File: {this.File}, TimeStamp: {this.TimeStamp}";
        }

        internal static FileInfo GetRestoreFileFor(FileInfo file, IBackupSettings setting)
        {
            var allBackups = GetAllBackupsFor(file, setting);
            if (!allBackups.Any())
            {
                return null;
            }

            return allBackups.MaxBy(x => x.TimeStamp).File;
        }

        internal static IList<BackupFile> GetAllBackupsFor(FileInfo file, IBackupSettings setting)
        {
            var pattern = GetBackupFilePattern(file, setting);
            var backups = setting.DirectoryPath.CreateDirectoryInfo().EnumerateFiles(pattern)
                                 .Select(x => new BackupFile(x, setting))
                                 .OrderBy(x => x.TimeStamp)
                                 .ToList();
            return backups;
        }

        internal static FileInfo CreateFor(FileInfo file, IBackupSettings setting)
        {
            var backup = file.WithNewExtension(setting.Extension)
                             .InDirectory(setting.DirectoryPath.CreateDirectoryInfo());

            if (string.IsNullOrEmpty(setting.TimeStampFormat))
            {
                return backup;
            }

            return backup.WithTimeStamp(DateTime.Now, setting);
        }

        private static string GetBackupFilePattern(FileInfo file, IBackupSettings setting)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName);
            var pattern = $"{fileNameWithoutExtension}*{setting.Extension}";
            return pattern;
        }
    }
}