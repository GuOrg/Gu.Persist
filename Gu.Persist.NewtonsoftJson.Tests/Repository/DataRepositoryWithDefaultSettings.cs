namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;

    using NUnit.Framework;

    public class DataRepositoryWithDefaultSettings : JsonRepositoryTests
    {
        [Test]
        public void DefaultSettings()
        {
            var settings = this.CreateRepository().Settings;
            Assert.AreEqual(".cfg", settings.Extension);
            Assert.AreEqual(this.Directory.FullName, settings.Directory);
            Assert.AreEqual(false, settings.IsTrackingDirty);
            Assert.AreEqual(".bak", settings.BackupSettings!.Extension);
            Assert.AreEqual(int.MaxValue, settings.BackupSettings.MaxAgeInDays);
            Assert.AreEqual(1, settings.BackupSettings.NumberOfBackups);
            Assert.AreEqual(null, settings.BackupSettings.TimeStampFormat);
        }

        protected override IRepository CreateRepository()
        {
            return new DataRepository(this.Directory);
        }
    }
}