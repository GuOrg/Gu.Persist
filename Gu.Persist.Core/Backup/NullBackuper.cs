namespace Gu.Persist.Core.Backup
{
    using System;
    using System.IO;

    /// <summary>
    /// A backuper that does not create any backups
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
        public void Backup(FileInfo file, FileInfo backup)
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
        public void AfterSuccessfulSave(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.ExtensionIsNot(file, FileHelper.SoftDeleteExtension, "file");
            file.DeleteSoftDeleteFileFor();
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo file, string newName)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            var soft = file.GetSoftDeleteFileFor();
            if (soft.Exists)
            {
                var fileSettings = new FileSettings(file.Directory, file.Extension);
                if (!soft.CanRename(newName, fileSettings))
                {
                    return false;
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
                var fileSettings = new FileSettings(file.Directory, file.Extension);
                var withNewName = soft.WithNewName(newName, fileSettings);
                soft.Rename(withNewName, owerWrite);
            }
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
                string message = $"Trying to restore {backup.FullName} when there is already an original: {file.FullName}";
                throw new InvalidOperationException(message);
            }

            FileHelper.Restore(file, backup);
        }
    }
}