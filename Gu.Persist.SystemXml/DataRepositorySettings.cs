namespace Gu.Persist.SystemXml
{
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    /// <summary>
    /// Settings used in <see cref="DataRepository"/>
    /// </summary>
    public class DataRepositorySettings : Core.DataRepositorySettings, IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositorySettings"/> class.
        /// </summary>
        public DataRepositorySettings(
            string directory,
            bool isTrackingDirty,
            bool saveNullDeletesFile,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, saveNullDeletesFile, backupSettings, extension, tempExtension)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositorySettings"/> class.
        /// Needed for serialization
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private DataRepositorySettings()
            : base(
                directory: Directories.AppDirectory().FullName,
                isTrackingDirty: false,
                saveNullDeletesFile: false,
                backupSettings: Default.BackupSettings(Directories.AppDirectory()))
        {
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            this.SetPrivate(nameof(this.Directory), reader.ReadElementString(nameof(this.Directory)));
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

        private static FieldInfo GetBackingField<T>(string propertyName)
        {
            return typeof(T).GetField(
                $"<{propertyName}>k__BackingField",
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void SetPrivate<T>(string propertyName, T value)
        {
            var field = GetBackingField<Core.DataRepositorySettings>(propertyName) ??
                        GetBackingField<Core.RepositorySettings>(propertyName) ??
                        GetBackingField<Core.FileSettings>(propertyName);
            Ensure.NotNull(field, nameof(field));
            field.SetValue(this, value);
        }
    }
}