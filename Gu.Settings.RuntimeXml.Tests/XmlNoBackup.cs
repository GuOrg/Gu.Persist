namespace Gu.Settings.RuntimeXml.Tests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Backup;
    using Gu.Settings.Core.Tests.Repositories;

    using NUnit.Framework;

    public class XmlNoBackup : RepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, Repository.Backuper);
        }

        protected override RepositorySettings Settings
        {
            get { return new RepositorySettings(Directory, null); }
        }

        protected override BackupSettings BackupSettings
        {
            get { return null; }
        }

        protected override IRepository Create()
        {
            return new XmlRepository(Settings);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            TestHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return TestHelper.Read<T>(file);
        }
    }
}