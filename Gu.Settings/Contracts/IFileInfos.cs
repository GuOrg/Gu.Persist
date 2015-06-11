namespace Gu.Settings
{
    using System.IO;

    public interface IFileInfos
    {
        FileInfo File { get; }

        /// <summary>
        /// Gets the temp file. This is the save that is saved to. On success it is renamed to File
        /// </summary>
        FileInfo TempFile { get; }

        /// <summary>
        /// Gets the backup file. If null no backups are made.
        /// </summary>
        FileInfo Backup { get; }
    }
}