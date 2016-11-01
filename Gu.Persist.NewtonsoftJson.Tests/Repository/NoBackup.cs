namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    public class SingletonRepositoryNoBackup : JsonRepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, this.Repository.Backuper);
        }

        protected override IRepository Create()
        {
            var settings = new NewtonsoftJson.RepositorySettings(
                this.TargetDirectory.FullName,
                NewtonsoftJson.RepositorySettings.CreateDefaultJsonSettings(),
                false,
                null);
            return new SingletonRepository(settings);
        }
    }
}