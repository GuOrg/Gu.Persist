namespace Gu.Persist.Core.Backup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// An <see cref="IBackuper"/> that saves copies.
    /// </summary>
    public class Backuper : IBackuper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Backuper"/> class.
        /// </summary>
        /// <param name="setting">The <see cref="IBackupSettings"/>.</param>
        protected Backuper(IBackupSettings setting)
        {
            this.Setting = setting ?? throw new ArgumentNullException(nameof(setting));
            new DirectoryInfo(setting.Directory).CreateIfNotExists();
        }

        /// <summary>
        /// Gets the <see cref="IBackupSettings"/>.
        /// </summary>
        public IBackupSettings Setting { get; }

#pragma warning disable CA1819 // Properties should not return arrays
        /// <summary>
        /// Gets the extensions for backup files and soft delete files.
        /// </summary>
        protected string[] BackupExtensions => new[] { this.Setting.Extension, FileHelper.SoftDeleteExtension };
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Creates a <see cref="Backuper"/> for the settings or <see cref="NullBackuper.Default"/> if <paramref name="setting"/> is null.
        /// </summary>
        /// <remarks>
        /// If <paramref name="setting"/> is null a <see cref="NullBackuper"/> is returned.
        /// </remarks>
        /// <param name="setting">The setting to use for backups.</param>
        /// <returns>An <see cref="IBackuper"/>.</returns>
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
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            Ensure.ExtensionIsNotAnyOf(file, this.BackupExtensions, nameof(file));
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
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.Backup(file.File);
        }

        /// <inheritdoc/>
        public virtual void Backup(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            Ensure.Exists(file, nameof(file));
            var backupFile = BackupFile.CreateFor(file, this.Setting);
            this.Backup(file, backupFile);
        }

        /// <inheritdoc/>
        public virtual void Backup(FileInfo file, FileInfo backup)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (backup is null)
            {
                throw new ArgumentNullException(nameof(backup));
            }

            Ensure.Exists(file, nameof(file));
            FileHelper.Backup(file, backup);
        }

        /// <inheritdoc/>
        public bool CanRestore(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

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
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

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
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public virtual void AfterSave(LockedFile file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

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
                    var backupToPurge = allBackups.MinBy(x => x.TimeStamp);
                    backupToPurge.File.HardDelete();
                    allBackups.Remove(backupToPurge);
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
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

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
        public void Rename(FileInfo file, string newName, bool overWrite)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            Ensure.NotNullOrEmpty(newName, nameof(newName));
            using (var transaction = new RenameTransaction(this.GetRenamePairs(file, newName)))
            {
                transaction.Commit(overWrite);
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<RenamePair> GetRenamePairs(FileInfo file, string newName)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            Ensure.NotNullOrEmpty(newName, nameof(newName));
            var pairs = new List<RenamePair>();
            var soft = file.GetSoftDeleteFileFor();
            if (soft.Exists)
            {
                var withNewName = soft.WithNewName(newName, new TempFileSettings(file.DirectoryName, file.Extension));
                pairs.Add(new RenamePair(soft, withNewName));
            }

            var allBackups = BackupFile.GetAllBackupsFor(file, this.Setting);
            foreach (var backup in allBackups)
            {
                var withNewName = backup.File.WithNewName(newName, this.Setting);
                pairs.Add(new RenamePair(backup.File, withNewName));
                soft = backup.File.GetSoftDeleteFileFor();
                if (soft.Exists)
                {
                    withNewName = soft.WithNewName(newName, this.Setting);
                    pairs.Add(new RenamePair(soft, withNewName));
                }
            }

            return pairs;
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
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

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

        /// <summary>
        /// Restore <paramref name="file"/> from <paramref name="backup"/>.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="backup">The backup <see cref="FileInfo"/>.</param>
        protected virtual void Restore(FileInfo file, FileInfo backup)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (backup is null)
            {
                throw new ArgumentNullException(nameof(backup));
            }

            Ensure.DoesNotExist(file, $"Trying to restore {backup.FullName} when there is already an original: {file.FullName}");
            backup.DeleteSoftDeleteFileFor();
            file.Restore(backup);
        }
    }
}
