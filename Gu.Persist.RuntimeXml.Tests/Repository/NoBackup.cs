namespace Gu.Persist.RuntimeXml.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    public class NoBackup : XmlRepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, this.Repository.Backuper);
        }

        protected override IRepository Create()
        {
            var settings = new RepositorySettings(this.Directory, null);
            return new SingletonRepository(settings);
        }
    }
}