namespace Gu.Settings.Backup
{
    using System.IO;

    public class NullBackuper : IBackuper
    {
        public IRepository Repository { get; set; }

        public void Backup(FileInfo file)
        {
            file.SoftDelete();
        }

        public void Backup(FileInfo file, FileInfo backup)
        {
            FileHelper.Backup(file, backup);
        }

        public void Restore(FileInfo file)
        {
          var backup =  file.RemoveExtension(FileHelper.SoftDeleteExtension);
            FileHelper.Restore(file, backup);
        }

        public void Restore(FileInfo file, FileInfo backup)
        {
            FileHelper.Restore(file, backup);
        }

        public void PurgeBackups(FileInfo file)
        {
            var backup = file.AppendExtension(FileHelper.SoftDeleteExtension);
            backup.HardDelete();
        }
    }
}