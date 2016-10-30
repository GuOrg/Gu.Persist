namespace Gu.Persist.RuntimeBinary.Tests
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Gu.Persist.Core;
    using Gu.Persist.RuntimeBinary;
    using NUnit.Framework;

    public class BinaryRepositorySettingsTests
    {
        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var backupSettings = new BackupSettings(new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name + @"\Backup"));
            var directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name);
            var settings = new BinaryRepositorySettings(directory, true, true, false, backupSettings, ".abc", ".cde");
            var repository = new BinaryRepository(settings);
            repository.Save(settings);
            var roundtripped = repository.Read<BinaryRepositorySettings>();
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var backupSettings = new BackupSettings(new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name + @"\Backup"));
            var directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name);
            var settings = new BinaryRepositorySettings(directory, true, true, false, backupSettings, ".abc", ".cde");
            var serializer = new BinaryFormatter();
            using (Stream stream = PooledMemoryStream.Borrow())
            {
                serializer.Serialize(stream, settings);
                stream.Position = 0;
                var roundtripped = (BinaryRepositorySettings)serializer.Deserialize(stream);
                AssertProperties(settings, roundtripped);
            }
        }

        [Test]
        public void RoundtripRepositorySettingsWithNullBackupSettings()
        {
            var directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name);
            var settings = new BinaryRepositorySettings(directory, true, true, true, null, ".abc", ".def");
            var serializer = new BinaryFormatter();
            using (Stream stream = PooledMemoryStream.Borrow())
            {
                serializer.Serialize(stream, settings);
                stream.Position = 0;
                var roundtripped = (BinaryRepositorySettings)serializer.Deserialize(stream);
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
