namespace Gu.Persist.NewtonsoftJson.Tests
{
    using System.IO;
    using Core;

    using Newtonsoft.Json;

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
            var backupSettings = BackupSettings.Create(backupDir, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new JsonRepositorySettings(
                this.directory,
                JsonRepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var json = JsonConvert.SerializeObject(settings);
            var roundtripped = JsonConvert.DeserializeObject<JsonRepositorySettings>(json);
            Assert.IsTrue(JsonEqualsComparer<JsonRepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettings()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var json = JsonConvert.SerializeObject(backupSettings);
            var roundtripped = JsonConvert.DeserializeObject<BackupSettings>(json);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettingsWithNullTimestampFormat()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", null, 2, 3);
            var json = JsonConvert.SerializeObject(backupSettings);
            var roundtripped = JsonConvert.DeserializeObject<BackupSettings>(json);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void RoundtripJsonRepositorySettingsWithRepository()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new JsonRepositorySettings(
                this.directory,
                JsonRepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var repository = new JsonRepository(settings);
            repository.Save(settings);
            var roundtripped = repository.Read<JsonRepositorySettings>();
            Assert.IsTrue(JsonEqualsComparer<JsonRepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new RepositorySettings(
                this.directory,
                false,
                false,
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var repository = new JsonRepository(this.directory);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettingsWithRepository()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new JsonRepositorySettings(
                this.directory,
                JsonRepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var repository = new JsonRepository(settings);
            repository.Save(backupSettings);
            var roundtripped = repository.Read<BackupSettings>();
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }
    }
}
