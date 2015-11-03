namespace Gu.Settings.SystemXml
{
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gu.Settings.Core;

    public class XmlRepositorySettings : RepositorySettings, IXmlSerializable
    {
        private XmlRepositorySettings()
        {
        }

        public XmlRepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
        }

        public XmlRepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
        }

        public XmlRepositorySettings(DirectoryInfo directory, bool isTrackingDirty, bool isCaching, BackupSettings backupSettings, string extension = ".cfg", string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, isCaching, backupSettings, extension, tempExtension)
        {
        }

        public static XmlRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new XmlRepositorySettings(directory, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            DirectoryPath = reader.ReadElementPathAndSpecialFolder(nameof(DirectoryPath));
            Extension = reader.ReadElementString(nameof(Extension));
            TempExtension = reader.ReadElementString(nameof(TempExtension));
            IsTrackingDirty = reader.ReadElementBool(nameof(IsTrackingDirty));
            IsCaching = reader.ReadElementBool(nameof(IsCaching));
            BackupSettings = reader.ReadElementBackupSettings(nameof(BackupSettings));
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(DirectoryPath), DirectoryPath);
            writer.WriteElementString(nameof(Extension), Extension);
            writer.WriteElementString(nameof(TempExtension), TempExtension);
            writer.WriteElementString(nameof(IsTrackingDirty), IsTrackingDirty);
            writer.WriteElementString(nameof(IsCaching), IsCaching);
            writer.WriteElementString(nameof(BackupSettings), BackupSettings);
        }
    }
}