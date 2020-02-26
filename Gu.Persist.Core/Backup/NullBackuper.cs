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
        /// <summary>
        /// The default instance.
        /// </summary>
        public static readonly NullBackuper Default = new NullBackuper();

        private NullBackuper()
        {
        }

        /// <inheritdoc/>
        public bool BeforeSave(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var softDelete = file.SoftDelete();
            return softDelete != null;
        }

        /// <inheritdoc/>
        void IBackuper.Backup(LockedFile file)
        {
            // nop;
        }

        /// <inheritdoc/>
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
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var softDelete = file.SoftDeleteFile();
            if (softDelete.Exists)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TryRestore(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

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
                    Restore(file, softDelete);
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
        public void AfterSave(LockedFile file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            Ensure.ExtensionIsNot(file.File, FileHelper.SoftDeleteExtension, "file");
            file.File.DeleteSoftDeleteFileFor();
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo file, string newName)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }

            var soft = file.SoftDeleteFile();
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
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }

            using var transaction = new RenameTransaction(this.GetRenamePairs(file, newName));
            transaction.Commit(overWrite);
        }

        /// <inheritdoc/>
        public IReadOnlyList<RenamePair> GetRenamePairs(FileInfo file, string newName)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }

            var soft = file.SoftDeleteFile();
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
            file.SoftDeleteFile()?.Delete();
        }

        private static void Restore(FileInfo file, FileInfo backup)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (backup is null)
            {
                throw new ArgumentNullException(nameof(backup));
            }

            file.Refresh();
            if (file.Exists)
            {
                var message = $"Trying to restore {backup.FullName} when there is already an original: {file.FullName}";
                throw new InvalidOperationException(message);
            }

            file.Restore(backup);
        }
    }
}