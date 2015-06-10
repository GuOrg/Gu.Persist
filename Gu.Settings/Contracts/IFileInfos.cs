namespace Gu.Settings
{
    using System.IO;

    public interface IFileInfos
    {
        FileInfo File { get; }

        /// <summary>
        /// Gets the backup file. If null no backups are made.
        /// </summary>
        FileInfo Backup { get; }
    }
}