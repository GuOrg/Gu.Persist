namespace Gu.Persist.Core
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Manages backups.
    /// </summary>
    public interface IBackuper
    {
        /// <summary>
        /// Creates a backup if <paramref name="file"/> exists.
        /// </summary>
        bool BeforeSave(FileInfo file);

        /// <summary>
        /// This method is called by the <see cref="SaveTransaction"/> after the copy to the temp file has finished and before the temp file is renamed to file.
        /// </summary>
        /// <param name="file">The file to create a backup for.</param>
        void Backup(LockedFile file);

        /// <summary>
        /// Creates a backup for <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The file to create a backup for.</param>
        void Backup(FileInfo file);

        /// <summary>
        /// Creates a backup for <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The file to create a backup for.</param>
        /// <param name="backup">The file to use as backup.</param>
        void Backup(FileInfo file, FileInfo backup);

        /// <summary>
        /// Checks if there is a soft delete file available or if there are backup(s).
        /// </summary>
        bool CanRestore(FileInfo file);

        /// <summary>
        /// Reads the newest backup if any.
        /// Order:
        /// 1) Soft delete file.
        /// 2) Newest backup if any.
        /// </summary>
        /// <returns>True if a backup was found and successfully restored. Now <paramref name="file"/> can be read.</returns>
        bool TryRestore(FileInfo file);

        // void Restore(FileInfo file); // Not trivially solved with singletons

        // void Restore(FileInfo file, FileInfo backup);

        /// <summary>
        /// This is called after <see cref="SaveTransaction"/> has renamed the temporary file to file.Name
        /// This means that the save transaction is complete and was successful.
        /// The files are still locked by the transaction.
        /// </summary>
        void AfterSave(LockedFile file);

        bool CanRename(FileInfo file, string newName);

        /// <summary>
        /// Rename the backup files for <paramref name="file"/> to.
        /// </summary>
        /// <param name="file">The file to rename backups for.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="overWrite">Overwrite newname if exists.</param>
        void Rename(FileInfo file, string newName, bool overWrite);

        /// <summary>
        /// Deletes all backups for <paramref name="file"/>.
        /// </summary>
        void DeleteBackups(FileInfo file);

        /// <summary>
        /// Rename the backup files for <paramref name="file"/> to.
        /// </summary>
        /// <param name="file">The file to rename backups for.</param>
        /// <param name="newName">The new name.</param>
        IReadOnlyList<RenamePair> GetRenamePairs(FileInfo file, string newName);
    }
}