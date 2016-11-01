namespace Gu.Persist.SystemXml.Tests
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    using NUnit.Framework;

    using RepositorySettings = Gu.Persist.SystemXml.RepositorySettings;

    public class RepositorySettingsTests
    {
        private static readonly BackupSettings BackupSettings =
            BackupSettings.Create(
                new DirectoryInfo(@"C:\Temp\Gu.Persist\Backup"),
                ".abc",
                BackupSettings.DefaultTimeStampFormat,
                1,
                2);

        private static readonly DirectoryInfo Directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\");

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var settings = new RepositorySettings(Directory, false, BackupSettings, ".cde", ".fgh");
            var repository = new SingletonRepository(settings);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var settings = new RepositorySettings(Directory, false, BackupSettings, ".cde", ".fgh");
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
                var roundtripped = (RepositorySettings)serializer.Deserialize(reader);
                AssertProperties(settings, roundtripped);
            }
        }

        [Test]
        public void RoundtripRepositorySettingsWithNullBackupSettings()
        {
            var settings = new RepositorySettings(Directory, true, null, ".cde", ".fgh");
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
                var roundtripped = (RepositorySettings)serializer.Deserialize(reader);
                AssertProperties(settings, roundtripped);
            }
        }

        [Test]
        public void CreateFromSelf()
        {
            var directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name);
            var settings = new RepositorySettings(directory, false, BackupSettings, ".abc", ".cde");
            var created = RepositorySettings.Create(settings);
            AssertProperties(settings, created);
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
