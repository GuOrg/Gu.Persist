namespace Gu.Persist.Core.Backup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Helper for managing backup files.
    /// </summary>
    public class BackupFile
    {
        private BackupFile(FileInfo file, IBackupSettings setting)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (setting is null)
            {
                throw new ArgumentNullException(nameof(setting));
            }

            Ensure.Exists(file);

            this.File = file;
            this.TimeStamp = file.GetTimeStamp(setting);
        }

        /// <summary>
        /// Gets the <see cref="FileInfo"/>.
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
            var backups = Directory.EnumerateFiles(setting.Directory, pattern)
                                   .Select(x => new BackupFile(new FileInfo(x), setting))
                                   .OrderBy(x => x.TimeStamp)
                                   .ToList();
            return backups;
        }

        internal static FileInfo CreateFor(FileInfo file, IBackupSettings setting)
        {
            var backup = file.WithNewExtension(setting.Extension)
                             .InDirectory(new DirectoryInfo(setting.Directory));

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