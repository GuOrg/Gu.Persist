namespace Gu.Persist.SystemXml
{
    using System.Xml;

    using Gu.Persist.Core;

    internal static class XmlWriterExt
    {
        internal static void WriteElementString(
            this XmlWriter writer,
            string elementName,
            BackupSettings backupSettings)
        {
            writer.WriteStartElement(elementName);
            writer.WriteElementString(nameof(backupSettings.Directory), backupSettings.Directory);
            writer.WriteElementString(nameof(backupSettings.Extension), backupSettings.Extension);
            writer.WriteElementString(nameof(backupSettings.TimeStampFormat), backupSettings.TimeStampFormat);
            writer.WriteElementString(nameof(backupSettings.NumberOfBackups), backupSettings.NumberOfBackups);
            writer.WriteElementString(nameof(backupSettings.MaxAgeInDays), backupSettings.MaxAgeInDays);
            writer.WriteEndElement();
        }

        internal static void WriteElementString(this XmlWriter writer, string elementName, bool value)
        {
            writer.WriteElementString(elementName, XmlConvert.ToString(value));
        }

        internal static void WriteElementString(this XmlWriter writer, string elementName, int value)
        {
            writer.WriteElementString(elementName, XmlConvert.ToString(value));
        }
    }
}