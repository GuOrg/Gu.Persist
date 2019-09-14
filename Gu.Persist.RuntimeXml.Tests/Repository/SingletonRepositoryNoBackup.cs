namespace Gu.Persist.RuntimeXml.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    public class SingletonRepositoryNoBackup : XmlRepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, this.Repository.Backuper);
        }

        protected override IRepository CreateRepository()
        {
            var settings = new RepositorySettings(
                directory: this.Directory.FullName,
                isTrackingDirty: false,
                backupSettings: null);
            return new SingletonRepository(settings);
        }
    }
}