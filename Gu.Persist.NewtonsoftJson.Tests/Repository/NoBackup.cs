namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    public class SingletonRepositoryNoBackup : JsonRepositoryTests
    {
        [Test]
        public void BackuperIsNullBackuperDefault()
        {
            Assert.AreSame(NullBackuper.Default, this.CreateRepository().Backuper);
        }

        protected override IRepository CreateRepository()
        {
            var settings = new NewtonsoftJson.RepositorySettings(
                directory: this.Directory.FullName,
                jsonSerializerSettings: NewtonsoftJson.RepositorySettings.CreateDefaultJsonSettings(),
                isTrackingDirty: false,
                backupSettings: null);
            return new SingletonRepository(settings);
        }
    }
}