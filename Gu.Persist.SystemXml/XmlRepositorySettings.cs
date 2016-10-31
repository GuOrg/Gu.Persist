namespace Gu.Persist.SystemXml
{
    using System.IO;
    using System.Reflection;
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
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : this(
                PathAndSpecialFolder.Create(directory),
                isTrackingDirty,
                saveNullDeletesFile,
                backupSettings,
                extension,
                tempExtension)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositorySettings"/> class.
        /// </summary>
        public XmlRepositorySettings(
            PathAndSpecialFolder directory,
            bool isTrackingDirty,
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(
                  directory,
                  isTrackingDirty,
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
            : base()
        {
        }

        /// <summary>
        /// A default instance for <paramref name="directory"/>
        /// </summary>
        public static XmlRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new XmlRepositorySettings(directory, true, false, BackupSettings.Create(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }

        public static XmlRepositorySettings Create(RepositorySettings settings)
        {
            return new XmlRepositorySettings(
                       settings.Directory,
                       settings.IsTrackingDirty,
                       settings.SaveNullDeletesFile,
                       settings.BackupSettings,
                       settings.Extension,
                       settings.TempExtension);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            this.SetPrivate(nameof(this.Directory), reader.ReadElementPathAndSpecialFolder(nameof(this.Directory)));
            this.SetPrivate(nameof(this.Extension), reader.ReadElementString(nameof(this.Extension)));
            this.SetPrivate(nameof(this.TempExtension), reader.ReadElementString(nameof(this.TempExtension)));
            this.SetPrivate(nameof(this.IsTrackingDirty), reader.ReadElementBool(nameof(this.IsTrackingDirty)));
            this.SetPrivate(nameof(this.SaveNullDeletesFile), reader.ReadElementBool(nameof(this.SaveNullDeletesFile)));
            this.SetPrivate(nameof(this.BackupSettings), reader.ReadElementBackupSettings(nameof(this.BackupSettings)));
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(this.Directory), this.Directory);
            writer.WriteElementString(nameof(this.Extension), this.Extension);
            writer.WriteElementString(nameof(this.TempExtension), this.TempExtension);
            writer.WriteElementString(nameof(this.IsTrackingDirty), this.IsTrackingDirty);
            writer.WriteElementString(nameof(this.SaveNullDeletesFile), this.SaveNullDeletesFile);
            if (this.BackupSettings != null)
            {
                writer.WriteElementString(nameof(this.BackupSettings), this.BackupSettings);
            }
        }

        private void SetPrivate<T>(string propertyName, T value)
        {
            var field = typeof(RepositorySettings)
                            .GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance) ??
                        typeof(FileSettings)
                            .GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            Ensure.NotNull(field, nameof(field));
            field.SetValue(this, value);
        }
    }
}