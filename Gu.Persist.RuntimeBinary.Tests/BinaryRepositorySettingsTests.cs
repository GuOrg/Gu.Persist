namespace Gu.Persist.RuntimeBinary.Tests
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Gu.Persist.Core;
    using Gu.Persist.RuntimeBinary;
    using NUnit.Framework;

    public class BinaryRepositorySettingsTests
    {
        private static readonly PathAndSpecialFolder Directory = new PathAndSpecialFolder(@"C:\Temp\Gu.Persist\", null);
        private static readonly PathAndSpecialFolder BackupDirectory = new PathAndSpecialFolder(@"C:\Temp\Gu.Persist\Backup\", null);

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var backupSettings = BackupSettings.Create(BackupDirectory);
            var settings = new RepositorySettings(Directory, false, backupSettings, ".abc", ".cde");
            var repository = new SingletonRepository(settings);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var backupSettings = BackupSettings.Create(BackupDirectory);
            var settings = new RepositorySettings(Directory, true, backupSettings, ".abc", ".cde");
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
            var settings = new RepositorySettings(Directory, true, null, ".abc", ".def");
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
                if (propertyInfo.PropertyType == typeof(BackupSettings) ||
                    propertyInfo.PropertyType == typeof(PathAndSpecialFolder))
                {
                    AssertProperties(expectedValue, actualValue);
                    continue;
                }

                Assert.AreEqual(expectedValue, actualValue, $"Not matching for {propertyInfo.Name}");
            }
        }
    }
}
