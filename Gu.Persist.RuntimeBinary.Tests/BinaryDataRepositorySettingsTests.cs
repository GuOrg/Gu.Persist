namespace Gu.Persist.RuntimeBinary.Tests
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gu.Persist.Core;

    using NUnit.Framework;

    public class BinaryDataRepositorySettingsTests
    {
        private static readonly DirectoryInfo Directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\");
        private static readonly DirectoryInfo BackupDirectory = new DirectoryInfo(@"C:\Temp\Gu.Persist\Backup\");
        private static readonly BackupSettings BackupSettings = new BackupSettings(
                                                                    BackupDirectory.FullName,
                                                                    ".abc",
                                                                    BackupSettings.DefaultTimeStampFormat,
                                                                    1,
                                                                    2);

        private static readonly DataRepositorySettings DataRepositorySettings = new DataRepositorySettings(
            directory: Directory.FullName,
            isTrackingDirty: false,
            saveNullDeletesFile: false,
            backupSettings: BackupSettings,
            extension: ".cde",
            tempExtension: ".fgh");

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var settings = new DataRepositorySettings(
                directory: Directory.FullName,
                isTrackingDirty: true,
                saveNullDeletesFile: true,
                backupSettings: BackupSettings,
                extension: ".cde",
                tempExtension: ".fgh");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<DataRepositorySettings>();
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var settings = new DataRepositorySettings(
                directory: Directory.FullName,
                isTrackingDirty: true,
                saveNullDeletesFile: true,
                backupSettings: BackupSettings,
                extension: ".cde",
                tempExtension: ".fgh");
            var serializer = new BinaryFormatter();
            using (Stream stream = PooledMemoryStream.Borrow())
            {
                serializer.Serialize(stream, settings);
                stream.Position = 0;
                var roundtripped = (DataRepositorySettings)serializer.Deserialize(stream);
                AssertProperties(settings, roundtripped);
            }
        }

        [Test]
        public void RoundtripRepositorySettingsWithNullBackupSettings()
        {
            var settings = new DataRepositorySettings(
                directory: Directory.FullName,
                isTrackingDirty: true,
                saveNullDeletesFile: true,
                backupSettings: BackupSettings,
                extension: ".cde",
                tempExtension: ".fgh");
            var serializer = new BinaryFormatter();
            using (Stream stream = PooledMemoryStream.Borrow())
            {
                serializer.Serialize(stream, settings);
                stream.Position = 0;
                var roundtripped = (DataRepositorySettings)serializer.Deserialize(stream);
                AssertProperties(settings, roundtripped);
            }
        }

        private static void AssertProperties(object expected, object actual)
        {
            if (ReferenceEquals(expected, actual))
            {
                return;
            }

            if (expected == null || actual == null)
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