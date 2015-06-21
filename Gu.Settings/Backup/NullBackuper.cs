namespace Gu.Settings.Backup
{
    using System;
    using System.ComponentModel;
    using System.IO;

    public class NullBackuper : IBackuper
    {
        public static readonly NullBackuper Default = new NullBackuper();

        protected NullBackuper()
        {
        }

        public virtual bool TryBackup(FileInfo file)
        {
            var softDelete = file.SoftDelete();
            return softDelete != null;
        }

        public virtual void Backup(FileInfo file, FileInfo backup)
        {
            FileHelper.Backup(file, backup);
        }

        public bool CanRestore(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            var softDelete = file.GetSoftDeleteFileFor();
            if (softDelete.Exists)
            {
                return true;
            }
            return false;
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
            catch (Exception)
            {
                return false;
            }
        }

        internal virtual void Restore(FileInfo file)
        {
            var softDelete = file.WithAppendedExtension(FileHelper.SoftDeleteExtension);
            if (softDelete.Exists)
            {
                Restore(file, softDelete);
            }
        }

        internal virtual void Restore(FileInfo file, FileInfo backup)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNull(backup, "backup");
            file.Refresh();
            if (file.Exists)
            {
                throw new InvalidOperationException(string.Format("Trying to restore {0} when there is already an original: {1}", backup.FullName, file.FullName));
            }
            FileHelper.Restore(file, backup);
        }

        public virtual void PurgeBackups(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            Ensure.ExtensionIsNot(file, FileHelper.SoftDeleteExtension, "file");
            file.DeleteSoftDeleteFileFor();
        }

        public bool CanRename(FileInfo file, string newName)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNullOrEmpty(newName, "newName");
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

        public void Rename(FileInfo file, string newName, bool owerWrite)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNullOrEmpty(newName, "newName");
            var soft = file.GetSoftDeleteFileFor();
            if (soft.Exists)
            {
                var fileSettings = new FileSettings(file.Directory, file.Extension);
                var withNewName = soft.WithNewName(newName, fileSettings);
                soft.Rename(withNewName, owerWrite);
            }
        }
    }
}