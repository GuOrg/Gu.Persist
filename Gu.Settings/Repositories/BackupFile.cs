namespace Gu.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Internals;

    public class BackupFile
    {
        public BackupFile(FileInfo file, BackupSettings setting)
        {
            File = file;
            TimeStamp = GetTimeStamp(file, setting);
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
            var directory = setting.Directory.FullName;
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
            string suffix = String.Empty;
            if (!String.IsNullOrWhiteSpace(setting.TimeStampFormat) || setting.NumberOfBackups > 1)
            {
                suffix = DateTime.Now.ToString(setting.TimeStampFormat, CultureInfo.InvariantCulture);
            }
            var backupName = Path.Combine(directory, String.Concat(fileNameWithoutExtension, suffix, setting.Extension));
            return new FileInfo(backupName);
        }


        internal static DateTime GetTimeStamp(FileInfo file, BackupSettings setting)
        {
            if (setting.TimeStampFormat == null)
            {
                return file.CreationTime;
            }
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName);
            var start = fileNameWithoutExtension.LastIndexOf(".");
            var length = fileNameWithoutExtension.Length - start;
            var substring = fileNameWithoutExtension.Substring(start, length);
            var timeStamp = DateTime.ParseExact(substring, setting.TimeStampFormat, CultureInfo.InvariantCulture);
            return timeStamp;
        }

        internal static string GetBackupFilePattern(FileInfo file, BackupSettings setting)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName);
            var pattern = String.Format("{0}*{1}", fileNameWithoutExtension, setting.Extension);
            return pattern;
        }
    }
}