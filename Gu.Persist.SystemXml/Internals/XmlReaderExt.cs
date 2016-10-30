namespace Gu.Persist.SystemXml
{
    using System;
    using System.Xml;

    using Gu.Persist.Core;

    internal static class XmlReaderExt
    {
        internal static PathAndSpecialFolder ReadElementPathAndSpecialFolder(this XmlReader reader, string elementName)
        {
            reader.ReadStartElement(elementName);
            var path = reader.ReadElementString(nameof(PathAndSpecialFolder.Path));
            var folder = reader.ReadElementNullableEnum<Environment.SpecialFolder>(nameof(PathAndSpecialFolder.SpecialFolder));
            reader.ReadEndElement();
            return new PathAndSpecialFolder(path, folder);
        }

        internal static BackupSettings ReadElementBackupSettings(this XmlReader reader, string elementName)
        {
            if (reader.Name != elementName)
            {
                return null;
            }

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return null;
            }

            reader.ReadStartElement(elementName);
            var directory = reader.ReadElementPathAndSpecialFolder(nameof(BackupSettings.DirectoryPath));
            var extension = reader.ReadElementString(nameof(BackupSettings.Extension));
            var isCreatingBackups = reader.ReadElementBool(nameof(BackupSettings.IsCreatingBackups));
            var timeStampFormat = reader.ReadElementString(nameof(BackupSettings.TimeStampFormat));
            var numberOfBackups = reader.ReadElementInt(nameof(BackupSettings.NumberOfBackups));
            var maxAgeInDays = reader.ReadElementInt(nameof(BackupSettings.MaxAgeInDays));
            reader.ReadEndElement();
            return new BackupSettings(directory, isCreatingBackups, extension, timeStampFormat, numberOfBackups, maxAgeInDays);
        }

        internal static bool ReadElementBool(this XmlReader reader, string elementName)
        {
            var text = reader.ReadElementString(elementName);
            var value = XmlConvert.ToBoolean(text);
            return value;
        }

        internal static int ReadElementInt(this XmlReader reader, string elementName)
        {
            var text = reader.ReadElementString(elementName);
            var value = XmlConvert.ToInt32(text);
            return value;
        }

        internal static T? ReadElementNullableEnum<T>(this XmlReader reader, string elementName)
            where T : struct
        {
            var value = reader.ReadElementString(elementName);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var e = (T)Enum.Parse(typeof(T), value);
            return e;
        }
    }
}