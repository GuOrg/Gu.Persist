namespace Gu.Persist.Yaml.Tests
{
    using System.IO;
    using System.Text;
    using Gu.Persist.Core;
    using NUnit.Framework;
    using YamlDotNet.Serialization;

    public class RepositorySettingsTests
    {
        private static readonly BackupSettings BackupSettings =
            new BackupSettings(
                @"C:\Temp\Gu.Persist\Backup",
                ".abc",
                BackupSettings.DefaultTimeStampFormat,
                1,
                2);

        private static readonly DirectoryInfo Directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\");

        private static readonly DataRepositorySettings DataRepositorySettings = new DataRepositorySettings(Directory.FullName, false, false, BackupSettings, ".cde", ".fgh");

        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var settings = new RepositorySettings(Directory.FullName, false, BackupSettings, ".cde", ".fgh");
            var repository = new DataRepository(DataRepositorySettings);
            repository.Save(settings);
            var roundtripped = repository.Read<RepositorySettings>();
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var settings = new RepositorySettings(Directory.FullName, false, BackupSettings, ".cde", ".fgh");
            var sb = new StringBuilder();
            var serializer = new Serializer();
            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, settings);
            }

            var xml = sb.ToString();

            ////Console.Write(xml);
            using (var reader = new StringReader(xml))
            {
                var roundtripped = new Deserializer().Deserialize<RepositorySettings>(reader);
                AssertProperties(settings, roundtripped);
            }
        }

        [Test]
        public void RoundtripRepositorySettingsWithNullBackupSettings()
        {
            var settings = new RepositorySettings(Directory.FullName, true, null, ".cde", ".fgh");
            var sb = new StringBuilder();
            var serializer = new Serializer();
            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, settings);
            }

            var xml = sb.ToString();

            ////Console.Write(xml);
            using (var reader = new StringReader(xml))
            {
                var roundtripped = new Deserializer().Deserialize<RepositorySettings>(reader);
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
