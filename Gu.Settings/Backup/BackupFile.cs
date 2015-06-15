namespace Gu.Settings.Backup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gu.Settings.Internals;

    public class BackupFile
    {
        public BackupFile(FileInfo file, BackupSettings setting)
        {
            File = file;
            TimeStamp = file.GetTimeStamp(setting);
        }

        public FileInfo File { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public override string ToString()
        {
            return string.Format("File: {0}, TimeStamp: {1}", File, TimeStamp);
        }

        internal static FileInfo GetRestoreFileFor(FileInfo file, BackupSettings setting)
        {
            var allBackups = GetAllBackupsFor(file, setting);
            return allBackups.MaxBy(x => x.TimeStamp).File;
        }

        internal static IList<BackupFile> GetAllBackupsFor(FileInfo file, BackupSettings setting)
        {
            var pattern = GetBackupFilePattern(file, setting);
            var backups = setting.Directory.EnumerateFiles(pattern)
                                 .Select(x => new BackupFile(x, setting))
                                 .OrderBy(x => x.TimeStamp)
                                 .ToList();
            return backups;
        }

        internal static FileInfo CreateFor(FileInfo file, BackupSettings setting)
        {
            var backup = file.ChangeExtension(setting.Extension);
            if (string.IsNullOrEmpty(setting.TimeStampFormat))
            {
                return file.ChangeExtension(setting.Extension);
            }
            return backup.AddTimeStamp(DateTime.Now, setting);
        }

        internal static string GetBackupFilePattern(FileInfo file, BackupSettings setting)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName);
            var pattern = String.Format("{0}*{1}", fileNameWithoutExtension, setting.Extension);
            return pattern;
        }
    }
}