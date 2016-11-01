namespace Gu.Persist.SystemXml.Tests
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Gu.Persist.Core;
    using Gu.Persist.SystemXml;

    using NUnit.Framework;

    using DataRepositorySettings = Gu.Persist.SystemXml.DataRepositorySettings;

    public class DataRepositorySettingsTests
    {
        private static readonly BackupSettings BackupSettings =
            BackupSettings.Create(
                new DirectoryInfo(@"C:\Temp\Gu.Persist\Backup"),
                ".abc",
                BackupSettings.DefaultTimeStampFormat,
                1,
                2);

        private static readonly PathAndSpecialFolder Directory = new PathAndSpecialFolder(@"C:\Temp\Gu.Persist\", null);

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var settings = new DataRepositorySettings(Directory, true, true, BackupSettings, ".cde", ".fgh");
            var repository = new DataRepository(settings);
            repository.Save(settings);
            var roundtripped = repository.Read<DataRepositorySettings>();
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var settings = new DataRepositorySettings(Directory, true, true, BackupSettings, ".cde", ".fgh");
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(settings.GetType());
            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, settings);
            }

            var xml = sb.ToString();

            ////Console.Write(xml);
            using (var reader = new StringReader(xml))
            {
                var roundtripped = (DataRepositorySettings)serializer.Deserialize(reader);
                AssertProperties(settings, roundtripped);
            }
        }

        [Test]
        public void RoundtripRepositorySettingsWithNullBackupSettings()
        {
            var settings = new DataRepositorySettings(Directory, true, true, null, ".cde", ".fgh");
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(settings.GetType());
            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, settings);
            }

            var xml = sb.ToString();

            ////Console.Write(xml);
            using (var reader = new StringReader(xml))
            {
               var roundtripped = (DataRepositorySettings)serializer.Deserialize(reader);
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