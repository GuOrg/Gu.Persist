namespace Gu.Persist.SystemXml
{
    using System;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    /// <summary>
    /// Settings used in <see cref="DataRepository"/>.
    /// </summary>
    public class DataRepositorySettings : Core.DataRepositorySettings, IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositorySettings"/> class.
        /// </summary>
        /// <param name="directory">The <see cref="string"/>.</param>
        /// <param name="isTrackingDirty">Configures if the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes.</param>
        /// <param name="saveNullDeletesFile">If saving null should throw or delete the file.</param>
        /// <param name="backupSettings">The <see cref="BackupSettings"/>.</param>
        /// <param name="extension">The file extension.</param>
        /// <param name="tempExtension">The temp file extension. Files are first written to temp files then changed extension for atomic writes.</param>
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
        /// Needed for serialization.
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

        /// <inheritdoc/>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <inheritdoc/>
        public void ReadXml(XmlReader reader)
        {
            if (reader is null)
            {
                throw new System.ArgumentNullException(nameof(reader));
            }

            reader.ReadStartElement();
            this.SetPrivate(nameof(this.Directory), reader.ReadElementString(nameof(this.Directory)));
            this.SetPrivate(nameof(this.Extension), reader.ReadElementString(nameof(this.Extension)));
            this.SetPrivate(nameof(this.TempExtension), reader.ReadElementString(nameof(this.TempExtension)));
            this.SetPrivate(nameof(this.IsTrackingDirty), reader.ReadElementBool(nameof(this.IsTrackingDirty)));
            this.SetPrivate(nameof(this.SaveNullDeletesFile), reader.ReadElementBool(nameof(this.SaveNullDeletesFile)));
            this.SetPrivate(nameof(this.BackupSettings), reader.ReadElementBackupSettings(nameof(this.BackupSettings)));
            reader.ReadEndElement();
        }

        /// <inheritdoc/>
        public void WriteXml(XmlWriter writer)
        {
            if (writer is null)
            {
                throw new System.ArgumentNullException(nameof(writer));
            }

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
                        GetBackingField<FileSettings>(propertyName);
            if (field is null)
            {
                throw new InvalidOperationException("Could not find backing field for " + propertyName);
            }

            field.SetValue(this, value);
        }
    }
}