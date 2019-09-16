namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.NewtonsoftJson;

    using NUnit.Framework;

    public class SingletonRepositoryWithDefaultSettings : JsonRepositoryTests
    {
        [Test]
        public void DefaultSettings()
        {
            var settings = this.CreateRepository().Settings;
            Assert.AreEqual(".cfg", settings.Extension);
            Assert.AreEqual(this.Directory.FullName, settings.Directory);
            Assert.AreEqual(false, settings.IsTrackingDirty);

            Assert.AreEqual(this.Directory.FullName, settings.BackupSettings.Directory);
            Assert.AreEqual(".bak", settings.BackupSettings.Extension);
            Assert.AreEqual(int.MaxValue, settings.BackupSettings.MaxAgeInDays);
            Assert.AreEqual(1, settings.BackupSettings.NumberOfBackups);
            Assert.AreEqual(null, settings.BackupSettings.TimeStampFormat);
        }

        protected override IRepository CreateRepository()
        {
            return new SingletonRepository(this.Directory);
        }
    }
}