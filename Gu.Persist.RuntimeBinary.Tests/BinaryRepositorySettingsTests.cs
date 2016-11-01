namespace Gu.Persist.RuntimeBinary.Tests
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Gu.Persist.Core;
    using Gu.Persist.RuntimeBinary;
    using NUnit.Framework;

    public class BinaryRepositorySettingsTests
    {
        private static readonly DirectoryInfo Directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\");
        private static readonly DirectoryInfo BackupDirectory = new DirectoryInfo(@"C:\Temp\Gu.Persist\Backup\");
        private static readonly BackupSettings BackupSettings = new BackupSettings(
                                                            BackupDirectory.FullName,
                                                            ".abc",
                                                            BackupSettings.DefaultTimeStampFormat,
                                                            1,
                                                            2);

        private static readonly DataRepositorySettings DataRepositorySettings = new DataRepositorySettings(Directory.FullName, false, false, BackupSettings, ".cde", ".fgh");

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var settings = new RepositorySettings(Directory.FullName, false, BackupSettings, ".abc", ".cde");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var settings = new RepositorySettings(Directory.FullName, true, BackupSettings, ".abc", ".cde");
            var serializer = new BinaryFormatter();
            using (Stream stream = PooledMemoryStream.Borrow())
            {
                serializer.Serialize(stream, settings);
                stream.Position = 0;
                var roundtripped = (RepositorySettings)serializer.Deserialize(stream);
                AssertProperties(settings, roundtripped);
            }
        }

        [Test]
        public void RoundtripRepositorySettingsWithNullBackupSettings()
        {
            var settings = new RepositorySettings(Directory.FullName, true, null, ".abc", ".def");
            var serializer = new BinaryFormatter();
            using (Stream stream = PooledMemoryStream.Borrow())
            {
                serializer.Serialize(stream, settings);
                stream.Position = 0;
                var roundtripped = (RepositorySettings)serializer.Deserialize(stream);
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
