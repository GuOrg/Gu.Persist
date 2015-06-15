namespace Gu.Settings.Backup
{
    using System;
    using System.IO;

    using Gu.Settings.Internals;

    public class Backuper : IBackuper
    {
        protected Backuper(BackupSettings setting)
        {
            Ensure.NotNull(setting, "setting");
            Setting = setting;
            setting.Directory.CreateIfNotExists();
        }

        public static readonly IBackuper None = new NullBackuper();

        public IRepository Repository { get; set; }

        public BackupSettings Setting { get; private set; }

        public static IBackuper Create(BackupSettings setting)
        {
            if (setting != null)
            {
                return new Backuper(setting);
            }
            return None;
        }

        public virtual void Backup<T>()
        {
            var file = FileHelper.CreateFileInfo<T>(Repository.Settings);
            Backup(file);
        }

        public virtual void Backup(FileInfo file)
        {
            if (!Setting.CreateBackups)
            {
                return;
            }
            var backupFile = BackupFile.CreateFor(file, Setting);
            Backup(file, backupFile);
        }

        public virtual void Backup(FileInfo file, FileInfo backup)
        {
            PurgeBackups(file);
            FileHelper.Backup(file, backup);
        }

        public virtual void Restore<T>()
        {
            var file = FileHelper.CreateFileInfo<T>(Repository.Settings);
            Restore(file);
        }

        public virtual void Restore(FileInfo file)
        {
            var backup = BackupFile.GetRestoreFileFor(file, Setting);
            Restore(file, backup);
        }

        public virtual void Restore(FileInfo file, FileInfo backup)
        {
            FileHelper.Restore(file, backup);
        }

        public virtual void PurgeBackups(FileInfo file)
        {
            var allBackups = BackupFile.GetAllBackupsFor(file, Setting);
            if (allBackups.Count == 0)
            {
                return;
            }
            if (Setting.NumberOfBackups > 0)
            {
                while (allBackups.Count > Setting.NumberOfBackups) // this is not efficient but the number of backups should be low
                {
                    var backupFile = allBackups.MinBy(x => x.TimeStamp);
                    backupFile.File.HardDelete();
                    allBackups.Remove(backupFile);
                }
            }
            if (Setting.MaxAgeInDays > 0 && Setting.MaxAgeInDays < Int32.MaxValue)
            {
                while (true) // this is not efficient but the number of backups should be low
                {
                    var backupFile = allBackups.MinBy(x => x.TimeStamp);
                    var days = (DateTime.Now - backupFile.TimeStamp).Days;
                    if (days < Setting.MaxAgeInDays)
                    {
                        break;
                    }
                    backupFile.File.HardDelete();
                    allBackups.Remove(backupFile);
                }
            }
        }

        [Obsolete("Implement")]
        private void MoveBackups<T>()
        {
        }
    }
}
