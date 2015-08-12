namespace Gu.Settings.Core.Backup
{
    using System;
    using System.IO;
    using System.Linq;

    using Gu.Settings.Core.Internals;

    public class Backuper : IBackuper
    {
        protected Backuper(BackupSettings setting)
        {
            Ensure.NotNull(setting, nameof(setting));
            Setting = setting;
            setting.Directory.CreateIfNotExists();
        }

        /// <summary>
        /// Gets the <see cref="IBackupSettings"/>
        /// </summary>
        public IBackupSettings Setting { get; }

        protected string[] BackupExtensions => new[] { Setting.Extension, FileHelper.SoftDeleteExtension };

        /// <summary>
        /// Creates a backuper for the given settings.
        /// </summary>
        /// <remarks>
        /// If <paramref name="setting"/> is null a <see cref="NullBackuper"/> is returned.
        /// </remarks>
        /// <param name="setting">The setting to use for backups.</param>
        /// <returns></returns>
        public static IBackuper Create(BackupSettings setting)
        {
            if (setting != null)
            {
                return new Backuper(setting);
            }
            return NullBackuper.Default;
        }

        /// <inheritdoc/>
        public virtual bool TryBackup(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNotAnyOf(file, BackupExtensions, "file");
            file.Refresh();
            if (!file.Exists)
            {
                return false;
            }

            if (!Setting.IsCreatingBackups)
            {
                var softDelete = file.SoftDelete();
                return softDelete != null;
            }
            var backupFile = BackupFile.CreateFor(file, Setting);
            Backup(file, backupFile);
            return true;
        }

        /// <inheritdoc/>
        public virtual void Backup(FileInfo file, FileInfo backup)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(backup, nameof(backup));
            FileHelper.Backup(file, backup);
        }

        /// <inheritdoc/>
        public bool CanRestore(FileInfo file)
        {
            var softDelete = file.GetSoftDeleteFileFor();
            if (softDelete.Exists)
            {
                return true;
            }
            var backups = BackupFile.GetAllBackupsFor(file, Setting);
            return backups.Any();
        }

        /// <inheritdoc/>
        public virtual bool TryRestore(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNotAnyOf(file, BackupExtensions, "file");
            Ensure.DoesNotExist(file);

            try
            {
                var softDelete = file.GetSoftDeleteFileFor();
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

        /// <inheritdoc/>
        internal virtual void Restore(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNotAnyOf(file, BackupExtensions, "file");

            var softDelete = file.WithAppendedExtension(FileHelper.SoftDeleteExtension);
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

        internal virtual void Restore(FileInfo file, FileInfo backup)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(backup, nameof(backup));
            Ensure.DoesNotExist(file, string.Format("Trying to restore {0} when there is already an original: {1}", backup.FullName, file.FullName));
            backup.DeleteSoftDeleteFileFor();
            FileHelper.Restore(file, backup);
        }

        /// <inheritdoc/>
        public virtual void PurgeBackups(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNotAnyOf(file, BackupExtensions, "file");
            file.DeleteSoftDeleteFileFor();
            var allBackups = BackupFile.GetAllBackupsFor(file, Setting);
            if (allBackups.Count == 0)
            {
                return;
            }

            foreach (var backup in allBackups)
            {
                backup.File.DeleteSoftDeleteFileFor();
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

        /// <inheritdoc/>
        public bool CanRename(FileInfo file, string newName)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            var soft = file.GetSoftDeleteFileFor();
            if (soft.Exists)
            {
                if (!soft.CanRename(newName, Setting))
                {
                    return false;
                }
            }

            var allBackups = BackupFile.GetAllBackupsFor(file, Setting);
            foreach (var backup in allBackups)
            {
                if (!backup.File.CanRename(newName, Setting))
                {
                    return false;
                }
                soft = backup.File.GetSoftDeleteFileFor();
                if (soft.Exists)
                {
                    if (!soft.CanRename(newName, Setting))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <inheritdoc/>
        public void Rename(FileInfo file, string newName, bool owerWrite)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            var soft = file.GetSoftDeleteFileFor();
            if (soft.Exists)
            {
                var withNewName = soft.WithNewName(newName, Setting);
                soft.Rename(withNewName, owerWrite);
            }

            var allBackups = BackupFile.GetAllBackupsFor(file, Setting);
            foreach (var backup in allBackups)
            {
                var withNewName = backup.File.WithNewName(newName, Setting);
                backup.File.Rename(withNewName, owerWrite);
                soft = backup.File.GetSoftDeleteFileFor();
                if (soft.Exists)
                {
                    withNewName = soft.WithNewName(newName, Setting);
                    soft.Rename(withNewName, owerWrite);
                }
            }
        }

        /// <inheritdoc/>
        public void DeleteBackups(FileInfo file)
        {
            var soft = file.GetSoftDeleteFileFor();
            if (soft != null)
            {
                soft.Delete();
            }
            var allBackups = BackupFile.GetAllBackupsFor(file, Setting);
            foreach (var backup in allBackups)
            {
                if (backup.File != null)
                {
                    backup.File.Delete();
                }
            }
        }

        [Obsolete("Implement")]
        private void MoveBackups<T>()
        {
        }
    }
}
