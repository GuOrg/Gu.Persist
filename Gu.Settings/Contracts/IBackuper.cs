namespace Gu.Settings
{
    using System.IO;

    public interface IBackuper
    {
        /// <summary>
        /// Creates a backup if file exists
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool TryBackup(FileInfo file);

        void Backup(FileInfo file, FileInfo backup);

        /// <summary>
        /// Checks if there is a soft delete file available or if there are backup(s)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool CanRestore(FileInfo file);

        /// <summary>
        /// Reads the newest backup if any.
        /// Order:
        /// 1) Soft delete file.
        /// 2) Newest backup if many.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>True if a backup was found and successfully restored. Now File can be read.</returns>
        bool TryRestore(FileInfo file);

        // void Restore(FileInfo file); // Not trivially solved with singletons

        // void Restore(FileInfo file, FileInfo backup);
        
        void PurgeBackups(FileInfo file);

        bool CanRename(FileInfo file, string newName);

        void Rename(FileInfo file, string newName, bool owerWrite);

        void DeleteBackups(FileInfo file);
    }
}