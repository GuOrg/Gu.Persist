namespace Gu.Settings.Backup
{
    using System.IO;

    public class NullBackuper : IBackuper
    {
        public static NullBackuper Default = new NullBackuper();

        protected NullBackuper()
        {
        }

        public virtual void Backup(FileInfo file)
        {
            file.SoftDelete();
        }

        public virtual void Backup(FileInfo file, FileInfo backup)
        {
            FileHelper.Backup(file, backup);
        }

        public virtual void Restore(FileInfo file)
        {
          var backup =  file.RemoveExtension(FileHelper.SoftDeleteExtension);
            FileHelper.Restore(file, backup);
        }

        public virtual void Restore(FileInfo file, FileInfo backup)
        {
            FileHelper.Restore(file, backup);
        }

        public virtual void PurgeBackups(FileInfo file)
        {
            var backup = file.AppendExtension(FileHelper.SoftDeleteExtension);
            backup.HardDelete();
        }
    }
}