namespace Gu.Persist.SystemXml.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    using RepositorySettings = Gu.Persist.SystemXml.RepositorySettings;

    public class NoBackup : XmlRepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, this.Repository.Backuper);
        }

        protected override IRepository Create()
        {
            var settings = new RepositorySettings(this.TargetDirectory, null);
            return new SingletonRepository(settings);
        }
    }
}