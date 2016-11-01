﻿namespace Gu.Persist.NewtonsoftJson.Tests
{
    using System.IO;
    using Core;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using DataRepositorySettings = Gu.Persist.NewtonsoftJson.DataRepositorySettings;
    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class SettingsTests
    {
        private static readonly DirectoryInfo Directory = new DirectoryInfo($@"C:\Temp\Gu.Persist\");
        private static readonly DirectoryInfo BackupDir = new DirectoryInfo($@"C:\Temp\Gu.Persist\Backup");
        private static readonly DataRepositorySettings DataRepositorySettings =new DataRepositorySettings(
                PathAndSpecialFolder.Create(Directory),
                RepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                null);

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(true);
        }

        [Test]
        public void RoundtripJsonRepositorySettings()
        {
            var backupSettings = BackupSettings.Create(BackupDir, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new RepositorySettings(
                PathAndSpecialFolder.Create(Directory),
                RepositorySettings.CreateDefaultJsonSettings(),
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
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var json = JsonConvert.SerializeObject(backupSettings);
            var roundtripped = JsonConvert.DeserializeObject<BackupSettings>(json);
            AssertProperties(backupSettings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new Core.RepositorySettings(
                               PathAndSpecialFolder.Create(Directory),
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
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", null, 2, 3);
            var json = JsonConvert.SerializeObject(backupSettings);
            var roundtripped = JsonConvert.DeserializeObject<BackupSettings>(json);
            AssertProperties(backupSettings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void RoundtripJsonRepositorySettingsWithRepository()
        {
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new RepositorySettings(
                PathAndSpecialFolder.Create(Directory),
                RepositorySettings.CreateDefaultJsonSettings(),
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripJsonDataRepositorySettingsWithRepository()
        {
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new NewtonsoftJson.DataRepositorySettings(
                PathAndSpecialFolder.Create(Directory),
                RepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<NewtonsoftJson.DataRepositorySettings>();
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<NewtonsoftJson.DataRepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new Core.RepositorySettings(
              PathAndSpecialFolder.Create(Directory),
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<Core.RepositorySettings>();
            AssertProperties(settings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<Core.RepositorySettings>.Default.Equals(settings, roundtripped));
        }

        [Test]
        public void RoundtripBackupSettingsWithRepository()
        {
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(backupSettings);
            var roundtripped = repository.Read<BackupSettings>();
            AssertProperties(backupSettings, roundtripped);
            Assert.IsTrue(JsonEqualsComparer<BackupSettings>.Default.Equals(backupSettings, roundtripped));
        }

        [Test]
        public void CreateFromSelf()
        {
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var settings = new RepositorySettings(
               PathAndSpecialFolder.Create(Directory),
                RepositorySettings.CreateDefaultJsonSettings(),
                false,
                backupSettings,
                ".cde",
                ".fgh");
            var created = RepositorySettings.Create(settings);
            AssertProperties(settings, created);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(settings, created));
        }

        [Test]
        public void CreateFromRepositorySettings()
        {
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var repositorySettings = new Core.RepositorySettings(
                                        PathAndSpecialFolder.Create(Directory),
                                         false,
                                         backupSettings,
                                         ".cde",
                                         ".fgh");
            var created = RepositorySettings.Create(repositorySettings);

            var jsonRepositorySettings = new RepositorySettings(
                PathAndSpecialFolder.Create(Directory),
                RepositorySettings.CreateDefaultJsonSettings(),
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
            var backupSettings = new BackupSettings(PathAndSpecialFolder.Create(BackupDir), ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var repositorySettings = new Core.RepositorySettings(
                                         PathAndSpecialFolder.Create(Directory),
                                         false,
                                         backupSettings,
                                         ".cde",
                                         ".fgh");
            var created = RepositorySettings.Create(repositorySettings, RepositorySettings.CreateDefaultJsonSettings());

            var jsonRepositorySettings = new RepositorySettings(
                PathAndSpecialFolder.Create(Directory),
                RepositorySettings.CreateDefaultJsonSettings(),
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
