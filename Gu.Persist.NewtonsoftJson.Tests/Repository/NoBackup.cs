namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class NoBackup : JsonRepositoryTests
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