namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    using Gu.Settings.Backup;

    using NUnit.Framework;

    public class BinaryNoBackup : RepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, Repository.Backuper);
        }

        protected override IRepository Create(RepositorySettings settings)
        {
            settings.BackupSettings = null;
            return new BinaryRepository(settings);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            BinaryHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return BinaryHelper.Read<T>(file);
        }
    }
}