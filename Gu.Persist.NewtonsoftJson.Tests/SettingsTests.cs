namespace Gu.Persist.NewtonsoftJson.Tests
{
    using System.IO;
    using Core;
    using NUnit.Framework;

    public class SettingsTests
    {
        private readonly DirectoryInfo directory;

        public SettingsTests()
        {
            this.directory = new DirectoryInfo($@"C:\Temp\Gu.Persist\{this.GetType().Name}");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.directory.Delete(true);
        }

        [Test]
        public void RoundtripJsonRepositorySettings()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(backupDir);
            var settings = new JsonRepositorySettings(
                this.directory,
                JsonRepositorySettings.CreateDefaultJsonSettings(),
                true,
                true,
                false,
                backupSettings,
                ".abc",
                ".cde");
            var repository = new JsonRepository(settings);
            repository.Save(settings);
            var roundtripped = repository.Read<JsonRepositorySettings>();
            Assert.IsTrue(JsonEqualsComparer<JsonRepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(backupDir);
            var settings = new RepositorySettings(
                this.directory,
                true,
                true,
                false,
                backupSettings,
                ".abc",
                ".cde");
            var repository = new JsonRepository(this.directory);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(settings, roundtripped));
        }
    }
}
