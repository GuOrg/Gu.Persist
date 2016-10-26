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
            this.DirectoryPath = reader.ReadElementPathAndSpecialFolder(nameof(this.DirectoryPath));
            this.Extension = reader.ReadElementString(nameof(this.Extension));
            this.TempExtension = reader.ReadElementString(nameof(this.TempExtension));
            this.IsTrackingDirty = reader.ReadElementBool(nameof(this.IsTrackingDirty));
            this.IsCaching = reader.ReadElementBool(nameof(this.IsCaching));
            this.BackupSettings = reader.ReadElementBackupSettings(nameof(this.BackupSettings));
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(this.DirectoryPath), this.DirectoryPath);
            writer.WriteElementString(nameof(this.Extension), this.Extension);
            writer.WriteElementString(nameof(this.TempExtension), this.TempExtension);
            writer.WriteElementString(nameof(this.IsTrackingDirty), this.IsTrackingDirty);
            writer.WriteElementString(nameof(this.IsCaching), this.IsCaching);
            writer.WriteElementString(nameof(this.BackupSettings), this.BackupSettings);
        }
    }
}