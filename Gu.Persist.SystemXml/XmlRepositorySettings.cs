namespace Gu.Persist.SystemXml
{
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    /// <summary>
    /// Specifies the behavior of a <see cref="XmlRepository"/>
    /// </summary>
    public class XmlRepositorySettings : RepositorySettings, IXmlSerializable
    {
        /// <summary> Initializes a new instance of the <see cref="XmlRepositorySettings"/> class. </summary>
        public XmlRepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositorySettings"/> class.
        /// </summary>
        public XmlRepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositorySettings"/> class.
        /// </summary>
        public XmlRepositorySettings(
            DirectoryInfo directory,
            bool isTrackingDirty,
            bool isCaching,
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(
                  directory,
                  isTrackingDirty,
                  isCaching,
                  saveNullDeletesFile,
                  backupSettings,
                  extension,
                  tempExtension)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositorySettings"/> class.
        /// Needed for serialization
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private XmlRepositorySettings()
        {
        }

        /// <summary>
        /// A default instance for <paramref name="directory"/>
        /// </summary>
        public static XmlRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new XmlRepositorySettings(directory, true, true, false, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            //this.DirectoryPath = reader.ReadElementPathAndSpecialFolder(nameof(this.DirectoryPath));
            //this.Extension = reader.ReadElementString(nameof(this.Extension));
            //this.TempExtension = reader.ReadElementString(nameof(this.TempExtension));
            //this.IsTrackingDirty = reader.ReadElementBool(nameof(this.IsTrackingDirty));
            //this.IsCaching = reader.ReadElementBool(nameof(this.IsCaching));
            //this.BackupSettings = reader.ReadElementBackupSettings(nameof(this.BackupSettings));
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
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