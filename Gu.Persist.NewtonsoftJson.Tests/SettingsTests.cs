namespace Gu.Persist.NewtonsoftJson.Tests
{
    using System.IO;
    using Core;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

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
            var settings = new RepositorySettings(
                this.directory,
                Core.RepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var json = JsonConvert.SerializeObject(settings);
            var roundtripped = JsonConvert.DeserializeObject<RepositorySettings>(json);
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettings()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var json = JsonConvert.SerializeObject(backupSettings);
            var roundtripped = JsonConvert.DeserializeObject<BackupSettings>(json);
            AssertProperties(backupSettings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new Core.RepositorySettings(
                               PathAndSpecialFolder.Create(this.directory),
                               false,
                               backupSettings,
                               ".cde",
                               ".fgh");
            var json = JsonConvert.SerializeObject(settings);
            var roundtripped = JsonConvert.DeserializeObject<Core.RepositorySettings>(json);
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<Core.RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettingsWithNullTimestampFormat()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", null, 2, 3);
            var json = JsonConvert.SerializeObject(backupSettings);
            var roundtripped = JsonConvert.DeserializeObject<BackupSettings>(json);
            AssertProperties(backupSettings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void RoundtripJsonRepositorySettingsWithRepository()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new RepositorySettings(
                this.directory,
                Core.RepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var repository = new SingletonRepository(settings);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new Core.RepositorySettings(
              PathAndSpecialFolder.Create(this.directory),
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var repository = new SingletonRepository(
                    new RepositorySettings(
                        this.directory,
                        Core.RepositorySettings.CreateDefaultJsonSettings(),
                        false,
                        false,
                        null,
                        ".cde",
                        ".fgh"));
            repository.Save(settings);
            var roundtripped = repository.Read<Core.RepositorySettings>();
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<Core.RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettingsWithRepository()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new RepositorySettings(
                this.directory,
                Core.RepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var repository = new SingletonRepository(settings);
            repository.Save(backupSettings);
            var roundtripped = repository.Read<BackupSettings>();
            AssertProperties(backupSettings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void CreateFromSelf()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new RepositorySettings(
                this.directory,
                Core.RepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var created = Core.RepositorySettings.Create(settings);
            AssertProperties(settings, created);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(settings, created));
        }

        [Test]
        public void CreateFromRepositorySettings()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var repositorySettings = new Core.RepositorySettings(
                                        PathAndSpecialFolder.Create(this.directory),
                                         false,
                                         backupSettings,
                                         ".cde",
                                         ".fgh");
            var created = Core.RepositorySettings.Create(repositorySettings);

            var jsonRepositorySettings = new RepositorySettings(
                               this.directory,
                               Core.RepositorySettings.CreateDefaultJsonSettings(),
                               false,
                               false,
                               backupSettings,
                               ".cde",
                               ".fgh");
            AssertProperties(jsonRepositorySettings, created);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(jsonRepositorySettings, created));
        }

        [Test]
        public void CreateFromRepositorySettingsAndJsonSettings()
        {
            var backupDir = this.directory.CreateSubdirectory("Backup");
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(backupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var repositorySettings = new Core.RepositorySettings(
                                         PathAndSpecialFolder.Create(this.directory),
                                         false,
                                         backupSettings,
                                         ".cde",
                                         ".fgh");
            var created = Core.RepositorySettings.Create(repositorySettings, Core.RepositorySettings.CreateDefaultJsonSettings());

            var jsonRepositorySettings = new RepositorySettings(
                               this.directory,
                               Core.RepositorySettings.CreateDefaultJsonSettings(),
                               false,
                               false,
                               backupSettings,
                               ".cde",
                               ".fgh");
            AssertProperties(jsonRepositorySettings, created);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(jsonRepositorySettings, created));
        }

        private static void AssertProperties<T>(T expected, T actual)
        {
            if (ReferenceEquals(expected, actual))
            {
                return;
            }

            if (expected == null || actual == null)
            {
                Assert.AreEqual(expected, actual);
            }

            // ReSharper disable once PossibleNullReferenceException
            foreach (var propertyInfo in expected.GetType().GetProperties())
            {
                var expectedValue = propertyInfo.GetValue(expected);
                var actualValue = propertyInfo.GetValue(actual);
                if (propertyInfo.PropertyType == typeof(BackupSettings) ||
                    propertyInfo.PropertyType == typeof(PathAndSpecialFolder) ||
                    propertyInfo.PropertyType == typeof(JsonSerializerSettings))
                {
                    AssertProperties(expectedValue, actualValue);
                    continue;
                }

                Assert.AreEqual(expectedValue, actualValue, $"Not matching for {propertyInfo.Name}");
            }
        }
    }
}
