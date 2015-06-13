namespace Gu.Settings
{
    using System.IO;

    public interface IBackuper
    {
        IRepository Repository { get; set; }

        void Backup(FileInfo file);

        void Backup(FileInfo file, FileInfo backup);

        void Restore(FileInfo file);

        void Restore(FileInfo file, FileInfo backup);
        void PurgeBackups(FileInfo file);
    }
}