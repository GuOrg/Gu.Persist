namespace Gu.Persist.Core.Backup
{
    using System;
    using System.IO;
    using System.Linq;

    public class Backuper : IBackuper
    {
        protected Backuper(IBackupSettings setting)
        {
            Ensure.NotNull(setting, nameof(setting));
            this.Setting = setting;
            setting.Directory.CreateDirectoryInfo().CreateIfNotExists();
        }

        /// <summary>
        /// Gets the <see cref="IBackupSettings"/>
        /// </summary>
        public IBackupSettings Setting { get; }

        protected string[] BackupExtensions => new[] { this.Setting.Extension, FileHelper.SoftDeleteExtension };

        /// <summary>
        /// Creates a backuper for the given settings.
        /// </summary>
        /// <remarks>
        /// If <paramref name="setting"/> is null a <see cref="NullBackuper"/> is returned.
        /// </remarks>
        /// <param name="setting">The setting to use for backups.</param>
        public static IBackuper Create(BackupSettings setting)
        {
            if (setting != null)
            {
                return new Backuper(setting);
            }

            return NullBackuper.Default;
        }

        /// <inheritdoc/>
        public virtual bool BeforeSave(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNotAnyOf(file, this.BackupExtensions, "file");
            file.Refresh();
            if (!file.Exists)
            {
                return false;
            }

            if (this.Setting == null)
            {
                var softDelete = file.SoftDelete();
                return softDelete != null;
            }

            return true;
        }

        /// <inheritdoc/>
        public virtual void Backup(LockedFile file)
        {
            this.Backup(file.File);
        }

        /// <inheritdoc/>
        public virtual void Backup(FileInfo file)
        {
            Ensure.Exists(file, nameof(file));
            var backupFile = BackupFile.CreateFor(file, this.Setting);
            this.Backup(file, backupFile);
        }

        /// <inheritdoc/>
        public virtual void Backup(FileInfo file, FileInfo backup)
        {
            Ensure.Exists(file, nameof(file));
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

            var backups = BackupFile.GetAllBackupsFor(file, this.Setting);
            return backups.Any();
        }

        /// <inheritdoc/>
        public virtual bool TryRestore(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNotAnyOf(file, this.BackupExtensions, "file");
            Ensure.DoesNotExist(file);

            try
            {
                var softDelete = file.GetSoftDeleteFileFor();
                if (softDelete.Exists)
                {
                    this.Restore(file, softDelete);
                    return true;
                }

                var backup = BackupFile.GetRestoreFileFor(file, this.Setting);
                if (backup != null)
                {
                    this.Restore(file, backup);
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
        public virtual void AfterSave(LockedFile file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNotAnyOf(file.File, this.BackupExtensions, "file");

            file.File.DeleteSoftDeleteFileFor();

            var allBackups = BackupFile.GetAllBackupsFor(file.File, this.Setting);
            if (allBackups.Count == 0)
            {
                return;
            }

            foreach (var backup in allBackups)
            {
                backup.File.DeleteSoftDeleteFileFor();
            }

            if (this.Setting.NumberOfBackups > 0)
            {
                // this is not efficient but the number of backups should be low
                while (allBackups.Count > this.Setting.NumberOfBackups)
                {
                    var backupToBurge = allBackups.MinBy(x => x.TimeStamp);
                    backupToBurge.File.HardDelete();
                    allBackups.Remove(backupToBurge);
                }
            }

            if (this.Setting.MaxAgeInDays > 0 && this.Setting.MaxAgeInDays < int.MaxValue)
            {
                // this is not efficient but the number of backups should be low
                while (true)
                {
                    var backupToBurge = allBackups.MinBy(x => x.TimeStamp);
                    var days = (DateTime.Now - backupToBurge.TimeStamp).Days;
                    if (days < this.Setting.MaxAgeInDays)
                    {
                        break;
                    }

                    backupToBurge.File.HardDelete();
                    allBackups.Remove(backupToBurge);
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
                if (!soft.CanRename(newName, this.Setting))
                {
                    return false;
                }
            }

            var allBackups = BackupFile.GetAllBackupsFor(file, this.Setting);
            foreach (var backup in allBackups)
            {
                if (!backup.File.CanRename(newName, this.Setting))
                {
                    return false;
                }

                soft = backup.File.GetSoftDeleteFileFor();
                if (soft.Exists)
                {
                    if (!soft.CanRename(newName, this.Setting))
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
                var withNewName = soft.WithNewName(newName, this.Setting);
                soft.Rename(withNewName, true);
            }

            var allBackups = BackupFile.GetAllBackupsFor(file, this.Setting);
            foreach (var backup in allBackups)
            {
                var withNewName = backup.File.WithNewName(newName, this.Setting);
                backup.File.Rename(withNewName, owerWrite);
                soft = backup.File.GetSoftDeleteFileFor();
                if (soft.Exists)
                {
                    withNewName = soft.WithNewName(newName, this.Setting);
                    soft.Rename(withNewName, owerWrite);
                }
            }
        }

        /// <inheritdoc/>
        public void DeleteBackups(FileInfo file)
        {
            var soft = file.GetSoftDeleteFileFor();
            soft?.Delete();

            var allBackups = BackupFile.GetAllBackupsFor(file, this.Setting);
            foreach (var backup in allBackups)
            {
                backup.File?.Delete();
            }
        }

        // ReSharper disable once UnusedMember.Global
        internal virtual void Restore(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNotAnyOf(file, this.BackupExtensions, "file");

            var softDelete = file.WithAppendedExtension(FileHelper.SoftDeleteExtension);
            if (softDelete.Exists)
            {
                this.Restore(file, softDelete);
                return;
            }

            var backup = BackupFile.GetRestoreFileFor(file, this.Setting);
            if (backup != null)
            {
                this.Restore(file, backup);
            }
        }

        protected virtual void Restore(FileInfo file, FileInfo backup)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(backup, nameof(backup));
            Ensure.DoesNotExist(file, $"Trying to restore {backup.FullName} when there is already an original: {file.FullName}");
            backup.DeleteSoftDeleteFileFor();
            FileHelper.Restore(file, backup);
        }
    }
}
