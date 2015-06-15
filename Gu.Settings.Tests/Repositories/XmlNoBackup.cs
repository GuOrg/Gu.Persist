namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    using Gu.Settings.Backup;

    using NUnit.Framework;

    public class XmlNoBackup : RepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, Repository.Backuper);
        }

        protected override IRepository Create(RepositorySettings settings)
        {
            settings.BackupSettings = null;
            return new XmlRepository(settings);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            XmlHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return XmlHelper.Read<T>(file);
        }
    }
}