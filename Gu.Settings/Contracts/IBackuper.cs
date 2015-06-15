namespace Gu.Settings
{
    using System.IO;

    public interface IBackuper
    {
        void Backup(FileInfo file);

        void Backup(FileInfo file, FileInfo backup);

        void Restore(FileInfo file);

        void Restore(FileInfo file, FileInfo backup);
        
        void PurgeBackups(FileInfo file);
    }
}