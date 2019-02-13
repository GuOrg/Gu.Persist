namespace Gu.Persist.Core.Backup
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// A backuper that does not create any backups.
    /// </summary>
    public sealed class NullBackuper : IBackuper
    {
        public static readonly NullBackuper Default = new NullBackuper();

        private NullBackuper()
        {
        }

        /// <inheritdoc/>
        public bool BeforeSave(FileInfo file)
        {
            var softDelete = file.SoftDelete();
            return softDelete != null;
        }

        /// <inheritdoc/>
        void IBackuper.Backup(LockedFile file)
        {
            // nop;
        }

        void IBackuper.Backup(FileInfo file)
        {
            // nop
        }

        /// <inheritdoc/>
        void IBackuper.Backup(FileInfo file, FileInfo backup)
        {
            FileHelper.Backup(file, backup);
        }

        /// <inheritdoc/>
        public bool CanRestore(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            var softDelete = file.GetSoftDeleteFileFor();
            if (softDelete.Exists)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TryRestore(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNot(file, FileHelper.SoftDeleteExtension, "file");
            Ensure.DoesNotExist(file);
            file.Refresh();
            if (file.Exists)
            {
                return false;
            }

            try
            {
                var softDelete = file.WithAppendedExtension(FileHelper.SoftDeleteExtension);
                if (softDelete.Exists)
                {
                    this.Restore(file, softDelete);
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
        public void AfterSave(LockedFile file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNot(file.File, FileHelper.SoftDeleteExtension, "file");
            file.File.DeleteSoftDeleteFileFor();
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo file, string newName)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            var soft = file.GetSoftDeleteFileFor();
            if (soft.Exists)
            {
                var fileSettings = new FileSettings(file.DirectoryName, file.Extension);
                if (!soft.CanRename(newName, fileSettings))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public void Rename(FileInfo file, string newName, bool overWrite)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            using (var transaction = new RenameTransaction(this.GetRenamePairs(file, newName)))
            {
                transaction.Commit(overWrite);
            }
        }

        public IReadOnlyList<RenamePair> GetRenamePairs(FileInfo file, string newName)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            var soft = file.GetSoftDeleteFileFor();
            if (soft.Exists)
            {
                var withNewName = soft.WithNewName(newName, new TempFileSettings(file.DirectoryName, file.Extension));
                return new[] { new RenamePair(soft, withNewName) };
            }

            return RenamePair.EmptyArray;
        }

        /// <inheritdoc/>
        public void DeleteBackups(FileInfo file)
        {
            var soft = file.GetSoftDeleteFileFor();
            soft?.Delete();
        }

        // ReSharper disable once UnusedMember.Global
        internal void Restore(FileInfo file)
        {
            var softDelete = file.WithAppendedExtension(FileHelper.SoftDeleteExtension);
            if (softDelete.Exists)
            {
                this.Restore(file, softDelete);
            }
        }

        internal void Restore(FileInfo file, FileInfo backup)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(backup, nameof(backup));
            file.Refresh();
            if (file.Exists)
            {
                var message = $"Trying to restore {backup.FullName} when there is already an original: {file.FullName}";
                throw new InvalidOperationException(message);
            }

            FileHelper.Restore(file, backup);
        }
    }
}