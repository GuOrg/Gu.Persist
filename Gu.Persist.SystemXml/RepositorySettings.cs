namespace Gu.Persist.SystemXml
{
    using System;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    /// <summary>
    /// Specifies the behavior of a <see cref="SingletonRepository"/>.
    /// </summary>
    public class RepositorySettings : Core.RepositorySettings, IXmlSerializable
    {
        /// <summary> Initializes a new instance of the <see cref="RepositorySettings"/> class. </summary>
        /// <param name="directory">The <see cref="string"/>.</param>
        /// <param name="isTrackingDirty">Configures if the repository keeps a cache of last saved/read bytes to use for comparing if instance has changes.</param>
        /// <param name="backupSettings">The <see cref="BackupSettings"/>.</param>
        /// <param name="extension">The file extension.</param>
        /// <param name="tempExtension">The temp file extension. Files are first written to temp files then changed extension for atomic writes.</param>
        public RepositorySettings(
            string directory,
            bool isTrackingDirty,
            BackupSettings? backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, backupSettings, extension, tempExtension)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// Needed for serialization.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private RepositorySettings()
            : base(
                directory: Directories.AppDirectory().FullName,
                isTrackingDirty: false,
                backupSettings: Default.BackupSettings(Directories.AppDirectory()))
        {
        }

        /// <inheritdoc/>
        public XmlSchema? GetSchema() => null;

        /// <inheritdoc/>
        public void ReadXml(XmlReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            reader.ReadStartElement();
            this.SetPrivate(nameof(this.Directory), reader.ReadElementString(nameof(this.Directory)));
            this.SetPrivate(nameof(this.Extension), reader.ReadElementString(nameof(this.Extension)));
            this.SetPrivate(nameof(this.TempExtension), reader.ReadElementString(nameof(this.TempExtension)));
            this.SetPrivate(nameof(this.IsTrackingDirty), reader.ReadElementBool(nameof(this.IsTrackingDirty)));
            this.SetPrivate(nameof(this.BackupSettings), reader.ReadElementBackupSettings(nameof(this.BackupSettings)));
            reader.ReadEndElement();
        }

        /// <inheritdoc/>
        public void WriteXml(XmlWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteElementString(nameof(this.Directory), this.Directory);
            writer.WriteElementString(nameof(this.Extension), this.Extension);
            writer.WriteElementString(nameof(this.TempExtension), this.TempExtension);
            writer.WriteElementString(nameof(this.IsTrackingDirty), this.IsTrackingDirty);
            if (this.BackupSettings != null)
            {
                writer.WriteElementString(nameof(this.BackupSettings), this.BackupSettings);
            }
        }

        private void SetPrivate<T>(string propertyName, T value)
        {
            var field = typeof(Core.RepositorySettings)
                            .GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance) ??
                        typeof(FileSettings)
                            .GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field is null)
            {
                throw new InvalidOperationException("Could not find backing field for " + propertyName);
            }

            field.SetValue(this, value);
        }
    }
}