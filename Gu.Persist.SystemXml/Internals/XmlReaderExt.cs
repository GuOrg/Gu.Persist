namespace Gu.Persist.SystemXml
{
    using System.Xml;

    using Gu.Persist.Core;

    internal static class XmlReaderExt
    {
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
            var directory = reader.ReadElementString(nameof(BackupSettings.Directory));
            var extension = reader.ReadElementString(nameof(BackupSettings.Extension));
            var timeStampFormat = reader.ReadElementString(nameof(BackupSettings.TimeStampFormat));
            var numberOfBackups = reader.ReadElementInt(nameof(BackupSettings.NumberOfBackups));
            var maxAgeInDays = reader.ReadElementInt(nameof(BackupSettings.MaxAgeInDays));
            reader.ReadEndElement();
            return new BackupSettings(directory, extension, timeStampFormat, numberOfBackups, maxAgeInDays);
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
    }
}