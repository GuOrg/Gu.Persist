namespace Gu.Persist.NewtonsoftJson.Tests
{
    using System.IO;

    using Gu.Persist.Core;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class SingletonRepositoryTests
    {
        private static readonly DirectoryInfo Directory = new DirectoryInfo($@"C:\Temp\Gu.Persist\");
        private static readonly DirectoryInfo BackupDir = new DirectoryInfo($@"C:\Temp\Gu.Persist\Backup");

        [Test]
        public void CreateFromRepositorySettings()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var repositorySettings = new Core.RepositorySettings(
                                         Directory.FullName,
                                         false,
                                         backupSettings,
                                         ".cde",
                                         ".fgh");
            var repository = new SingletonRepository(repositorySettings);

            var jsonRepositorySettings = new RepositorySettings(
                                             Directory.FullName,
                                             RepositorySettings.CreateDefaultJsonSettings(),
                                             false,
                                             backupSettings,
                                             ".cde",
                                             ".fgh");
            AssertProperties(jsonRepositorySettings, repository.Settings);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(jsonRepositorySettings, repository.Settings));
        }

        [Test]
        public void CreateFromRepositorySettingsAndJsonSettings()
        {
            var backupSettings = new BackupSettings(BackupDir.FullName, ".abc", BackupSettings.DefaultTimeStampFormat, 2, 3);
            var repositorySettings = new Core.RepositorySettings(
                                         Directory.FullName,
                                         false,
                                         backupSettings,
                                         ".cde",
                                         ".fgh");
            var created = new SingletonRepository(repositorySettings, RepositorySettings.CreateDefaultJsonSettings());

            var jsonRepositorySettings = new RepositorySettings(
                                             Directory.FullName,
                                             RepositorySettings.CreateDefaultJsonSettings(),
                                             false,
                                             backupSettings,
                                             ".cde",
                                             ".fgh");
            AssertProperties(jsonRepositorySettings, created.Settings);
            Assert.IsTrue(JsonEqualsComparer<RepositorySettings>.Default.Equals(jsonRepositorySettings, created.Settings));
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