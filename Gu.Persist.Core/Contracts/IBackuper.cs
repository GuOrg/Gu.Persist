namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// Manages backups
    /// </summary>
    public interface IBackuper
    {
        /// <summary>
        /// Creates a backup if <paramref name="file"/> exists
        /// </summary>
        bool BeforeSave(FileInfo file);

        /// <summary>
        /// Creates a backup for <paramref name="file"/>
        /// </summary>
        /// <param name="file">The file to create a backup for</param>
        /// <param name="backup">The file to use as backup.</param>
        void Backup(FileInfo file, FileInfo backup);

        /// <summary>
        /// Checks if there is a soft delete file available or if there are backup(s)
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
        /// Removes old backups for <paramref name="file"/>
        /// </summary>
        void AfterSuccessfulSave(FileInfo file);

        bool CanRename(FileInfo file, string newName);

        void Rename(FileInfo file, string newName, bool owerWrite);

        /// <summary>
        /// Deletes all backups for <paramref name="file"/>
        /// </summary>
        void DeleteBackups(FileInfo file);
    }
}