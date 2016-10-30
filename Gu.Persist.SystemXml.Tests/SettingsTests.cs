﻿namespace Gu.Persist.SystemXml.Tests
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    using NUnit.Framework;

    public class XmlRepositorySettingsTests
    {
        [Test]
        public void RoundtripRepositorySettingsWithRepository()
        {
            var backupSettings = new BackupSettings(new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name + @"\Backup"));
            var directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name);
            var settings = new XmlRepositorySettings(directory, true, true, false, backupSettings, ".cfg", ".tmp");
            var repository = new XmlRepository(settings);
            repository.Save(settings);
            var roundtripped = repository.Read<XmlRepositorySettings>();
            AssertProperties(settings, roundtripped);
        }

        [Test]
        public void RoundtripRepositorySettings()
        {
            var backupSettings = new BackupSettings(new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name + @"\Backup"));
            var directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name);
            var settings = new XmlRepositorySettings(directory, true, true, false, backupSettings, ".cfg", ".tmp");
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(settings.GetType());
            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, settings);
            }

            var xml = sb.ToString();

            ////Console.Write(xml);
            XmlRepositorySettings roundtripped;

            using (var reader = new StringReader(xml))
            {
                roundtripped = (XmlRepositorySettings)serializer.Deserialize(reader);
            }

            AssertProperties(settings, roundtripped);
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
