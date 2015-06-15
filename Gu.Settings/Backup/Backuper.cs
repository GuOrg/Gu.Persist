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

        public IBackupSettings Setting { get; private set; }

        protected string[] BackupExtensions
        {
            get { return new[] { Setting.Extension, FileHelper.SoftDeleteExtension }; } // this can be local field if allocations become a problem which is unlikely.
        }

        public static IBackuper Create(BackupSettings setting)
        {
            if (setting != null)
            {
                return new Backuper(setting);
            }
            return NullBackuper.Default;
        }

        public virtual bool TryBackup(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            Ensure.ExtensionIsNotAnyOf(file, BackupExtensions, "file");
            file.Refresh();
            if (!file.Exists)
            {
                return false;
            }

            if (!Setting.CreateBackups)
            {
                var softDelete = file.SoftDelete();
                return softDelete != null;
            }
            var backupFile = BackupFile.CreateFor(file, Setting);
            Backup(file, backupFile);
            return true;
        }

        public virtual void Backup(FileInfo file, FileInfo backup)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNull(backup, "backup");
            FileHelper.Backup(file, backup);
        }

        /// <summary>
        /// Reads the newest backup if any.
        /// Order:
        /// 1) Soft delete file.
        /// 2) Newest backup if many.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>True if a backup was found and successfully restored. Now File can be read.</returns>
        public virtual bool TryRestore(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            Ensure.ExtensionIsNotAnyOf(file, BackupExtensions, "file");
            Ensure.DoesNotExist(file);

            try
            {
                var softDelete = file.AppendExtension(FileHelper.SoftDeleteExtension);
                if (softDelete.Exists)
                {
                    Restore(file, softDelete);
                    return true;
                }

                var backup = BackupFile.GetRestoreFileFor(file, Setting);
                if (backup != null)
                {
                    Restore(file, backup);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual void Restore(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            Ensure.ExtensionIsNotAnyOf(file, BackupExtensions, "file");
          
            var softDelete = file.AppendExtension(FileHelper.SoftDeleteExtension);
            if (softDelete.Exists)
            {
                Restore(file, softDelete);
                return;
            }

            var backup = BackupFile.GetRestoreFileFor(file, Setting);
            if (backup != null)
            {
                Restore(file, backup);
            }
        }

        public virtual void Restore(FileInfo file, FileInfo backup)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNull(backup, "backup");
            Ensure.DoesNotExist(file, string.Format("Trying to restore {0} when there is already an original: {1}", backup.FullName, file.FullName));
 
            FileHelper.Restore(file, backup);
        }

        public virtual void PurgeBackups(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            Ensure.ExtensionIsNotAnyOf(file, BackupExtensions, "file");
            var softDelete = file.AppendExtension(FileHelper.SoftDeleteExtension);
            softDelete.Delete();
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
