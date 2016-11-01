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
            Assert.AreEqual(".cfg", this.Settings.Extension);
            Assert.AreEqual(this.TargetDirectory.FullName, this.Settings.Directory);
            Assert.AreEqual(false, this.Settings.IsTrackingDirty);

            Assert.AreEqual(this.TargetDirectory.FullName, this.Settings.BackupSettings.Directory);
            Assert.AreEqual(".bak", this.Settings.BackupSettings.Extension);
            Assert.AreEqual(int.MaxValue, this.Settings.BackupSettings.MaxAgeInDays);
            Assert.AreEqual(1, this.Settings.BackupSettings.NumberOfBackups);
            Assert.AreEqual(null, this.Settings.BackupSettings.TimeStampFormat);
        }

        protected override IRepository Create()
        {
            return new SingletonRepository(this.TargetDirectory);
        }
    }
}