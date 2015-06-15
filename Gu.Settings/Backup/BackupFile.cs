namespace Gu.Settings.Backup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gu.Settings.Internals;

    public class BackupFile
    {
        public BackupFile(FileInfo file, IBackupSettings setting)
        {
            Ensure.Exists(file);
            Ensure.NotNull(setting, "setting");

            File = file;
            TimeStamp = file.GetTimeStamp(setting);
        }

        public FileInfo File { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public override string ToString()
        {
            return string.Format("File: {0}, TimeStamp: {1}", File, TimeStamp);
        }

        internal static FileInfo GetRestoreFileFor(FileInfo file, IBackupSettings setting)
        {
            Ensure.ExtensionIsNot(file, setting.Extension, "file");
            Ensure.ExtensionIsNot(file, FileHelper.SoftDeleteExtension, "file");
            Ensure.NotNull(setting, "setting");

            var allBackups = GetAllBackupsFor(file, setting);
            if (!allBackups.Any())
            {
                return null;
            }
            return allBackups.MaxBy(x => x.TimeStamp).File;
        }

        internal static IList<BackupFile> GetAllBackupsFor(FileInfo file, IBackupSettings setting)
        {
            Ensure.ExtensionIsNot(file, setting.Extension, "file");
            Ensure.ExtensionIsNot(file, FileHelper.SoftDeleteExtension, "file");
            Ensure.NotNull(setting, "setting");

            var pattern = GetBackupFilePattern(file, setting);
            var backups = setting.Directory.EnumerateFiles(pattern)
                                 .Select(x => new BackupFile(x, setting))
                                 .OrderBy(x => x.TimeStamp)
                                 .ToList();
            return backups;
        }

        internal static FileInfo CreateFor(FileInfo file, IBackupSettings setting)
        {
            Ensure.Exists(file);
            Ensure.ExtensionIsNot(file, setting.Extension, "file");
            Ensure.ExtensionIsNot(file, FileHelper.SoftDeleteExtension, "file");
            Ensure.NotNull(setting, "setting");

            var backup = file.ChangeExtension(setting.Extension);
            if (string.IsNullOrEmpty(setting.TimeStampFormat))
            {
                return file.ChangeExtension(setting.Extension);
            }
            return backup.AddTimeStamp(DateTime.Now, setting);
        }

        internal static string GetBackupFilePattern(FileInfo file, IBackupSettings setting)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNull(setting, "setting");

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName);
            var pattern = String.Format("{0}*{1}", fileNameWithoutExtension, setting.Extension);
            return pattern;
        }
    }
}