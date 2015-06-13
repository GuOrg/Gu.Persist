namespace Gu.Settings
{
    using System;
    using System.IO;

    public class NullBackuper : IBackuper
    {
        public IRepository Repository
        {
            get { return null; }
            set { throw new InvalidOperationException();}
        }

        public void Backup(FileInfo file)
        {
        }

        public void Backup(FileInfo file, FileInfo backup)
        {
        }

        public void Restore(FileInfo file)
        {
        }

        public void Restore(FileInfo file, FileInfo backup)
        {
        }

        public void PurgeBackups(FileInfo file)
        {
        }
    }
}