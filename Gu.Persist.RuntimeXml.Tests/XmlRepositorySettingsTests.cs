namespace Gu.Persist.RuntimeXml.Tests
{
    using System.IO;
    using System.Runtime.Serialization;
    using Gu.Persist.Core;
    using Gu.Persist.RuntimeXml;
    using NUnit.Framework;

    public class XmlRepositorySettingsTests
    {
        private static readonly BackupSettings BackupSettings = new(
            @"C:\Temp\Gu.Persist\Backup",
            ".abc",
            BackupSettings.DefaultTimeStampFormat,
            1,
            2);

        private static readonly DirectoryInfo Directory = new(@"C:\Temp\Gu.Persist\");

        private static readonly DataRepositorySettings DataRepositorySettings = new(
            directory: Directory.FullName,
            isTrackingDirty: false,
            saveNullDeletesFile: false,
            backupSettings: BackupSettings);

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var settings = new RepositorySettings(
                directory: Directory.FullName,
                isTrackingDirty: false,
                backupSettings: BackupSettings,
                extension: ".cde",
                tempExtension: ".fgh");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var settings = new RepositorySettings(
                directory: Directory.FullName,
                isTrackingDirty: true,
                backupSettings: BackupSettings,
                extension: ".cde",
                tempExtension: ".fgh");
            var serializer = new DataContractSerializer(settings.GetType());
            using Stream stream = PooledMemoryStream.Borrow();
            serializer.WriteObject(stream, settings);
            stream.Position = 0;
            var roundtripped = (RepositorySettings)serializer.ReadObject(stream);
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettingsWithNullBackupSettings()
        {
            var settings = new RepositorySettings(
                directory: Directory.FullName,
                isTrackingDirty: true,
                backupSettings: null,
                extension: ".cde",
                tempExtension: ".fgh");
            var serializer = new DataContractSerializer(settings.GetType());
            using Stream stream = PooledMemoryStream.Borrow();
            serializer.WriteObject(stream, settings);
            stream.Position = 0;
            var roundtripped = (RepositorySettings)serializer.ReadObject(stream);
            AssertProperties(settings, roundtripped);
        }

        private static void AssertProperties(object? expected, object? actual)
        {
            if (ReferenceEquals(expected, actual))
            {
                return;
            }

            if (expected is null || actual is null)
            {
                throw new AssertionException("Assert failed");
            }

            foreach (var propertyInfo in expected.GetType().GetProperties())
            {
                var expectedValue = propertyInfo.GetValue(expected);
                var actualValue = propertyInfo.GetValue(actual);
                if (propertyInfo.PropertyType == typeof(BackupSettings))
                {
                    AssertProperties(expectedValue, actualValue);
                    continue;
                }

                Assert.AreEqual(expectedValue, actualValue, $"Not matching for {propertyInfo.Name}");
            }
        }
    }
}
