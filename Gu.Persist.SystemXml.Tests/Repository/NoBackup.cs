namespace Gu.Persist.SystemXml.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    using RepositorySettings = Gu.Persist.SystemXml.RepositorySettings;

    public class SingletonRepositoryNoBackup : XmlRepositoryTests
    {
        [Test]
        public void BackuperIsNullBackuperDefault()
        {
            Assert.AreSame(NullBackuper.Default, this.CreateRepository().Backuper);
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