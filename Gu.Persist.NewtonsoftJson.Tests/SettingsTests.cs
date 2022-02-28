namespace Gu.Persist.NewtonsoftJson.Tests
{
    using System.IO;
    using Gu.Persist.Core;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using DataRepositorySettings = Gu.Persist.NewtonsoftJson.DataRepositorySettings;
    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class SettingsTests
    {
        private static readonly DirectoryInfo Directory = new($@"C:\Temp\Gu.Persist\");
        private static readonly DirectoryInfo BackupDir = new($@"C:\Temp\Gu.Persist\Backup");

        private static readonly DataRepositorySettings DataRepositorySettings = new(
            directory: Directory.FullName,
            jsonSerializerSettings: RepositorySettings.CreateDefaultJsonSettings(),
            isTrackingDirty: false,
            saveNullDeletesFile: false,
            backupSettings: null);

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(recursive: true);
        }

        [Test]
        public void RoundtripJsonRepositorySettings()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new RepositorySettings(
                directory: Directory.FullName,
                jsonSerializerSettings: RepositorySettings.CreateDefaultJsonSettings(),
                isTrackingDirty: false,
                backupSettings: backupSettings,
                extension: ".cde",
                tempExtension: ".fgh");
            var json = JsonConvert.SerializeObject(settings);
            var roundtripped = JsonConvert.DeserializeObject<RepositorySettings>(json);
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettings()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var json = JsonConvert.SerializeObject(backupSettings);
            var roundtripped = JsonConvert.DeserializeObject<BackupSettings>(json);
            AssertProperties(backupSettings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new Core.RepositorySettings(
                               directory: Directory.FullName,
                               isTrackingDirty: false,
                               backupSettings: backupSettings,
                               extension: ".cde",
                               tempExtension: ".fgh");
            var json = JsonConvert.SerializeObject(settings);
            var roundtripped = JsonConvert.DeserializeObject<Core.RepositorySettings>(json);
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<Core.RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettingsWithNullTimestampFormat()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", null, 2, 3);
            var json = JsonConvert.SerializeObject(backupSettings);
            var roundtripped = JsonConvert.DeserializeObject<BackupSettings>(json);
            AssertProperties(backupSettings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void RoundtripJsonRepositorySettingsWithRepository()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new RepositorySettings(
                directory: Directory.FullName,
                jsonSerializerSettings: RepositorySettings.CreateDefaultJsonSettings(),
                isTrackingDirty: false,
                backupSettings: backupSettings,
                extension: ".cde",
                tempExtension: ".fgh");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripJsonDataRepositorySettingsWithRepository()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new DataRepositorySettings(
                directory: Directory.FullName,
                jsonSerializerSettings: RepositorySettings.CreateDefaultJsonSettings(),
                isTrackingDirty: false,
                saveNullDeletesFile: false,
                backupSettings: backupSettings,
                extension: ".cde",
                tempExtension: ".fgh");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<DataRepositorySettings>();
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<DataRepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new Core.RepositorySettings(
                directory: Directory.FullName,
                isTrackingDirty: false,
                backupSettings: backupSettings,
                extension: ".cde",
                tempExtension: ".fgh");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<Core.RepositorySettings>();
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<Core.RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettingsWithRepository()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(backupSettings);
            var roundtripped = repository.Read<BackupSettings>();
            AssertProperties(backupSettings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        private static void AssertProperties<T>(T expected, T actual)
        {
            if (ReferenceEquals(expected, actual))
            {
                return;
            }

            if (expected is null || actual is null)
            {
                Assert.AreEqual(expected, actual);
            }

            // ReSharper disable once PossibleNullReferenceException
            foreach (var propertyInfo in expected!.GetType().GetProperties())
            {
                var expectedValue = propertyInfo.GetValue(expected);
                var actualValue = propertyInfo.GetValue(actual);
                if (propertyInfo.PropertyType == typeof(BackupSettings) ||
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
