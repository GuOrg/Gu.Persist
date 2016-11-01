namespace Gu.Persist.SystemXml
{
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    /// <summary>
    /// Specifies the behavior of a <see cref="SingletonRepository"/>
    /// </summary>
    public class RepositorySettings : Core.RepositorySettings, IXmlSerializable
    {
        /// <summary> Initializes a new instance of the <see cref="RepositorySettings"/> class. </summary>
        public RepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="RepositorySettings"/> class. </summary>
        public RepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="RepositorySettings"/> class. </summary>
        public RepositorySettings(
            DirectoryInfo directory,
            bool isTrackingDirty,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : this(
                PathAndSpecialFolder.Create(directory),
                isTrackingDirty,
                backupSettings,
                extension,
                tempExtension)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="RepositorySettings"/> class. </summary>
        public RepositorySettings(
            PathAndSpecialFolder directory,
            bool isTrackingDirty,
            BackupSettings backupSettings,
            string extension = ".cfg",
            string tempExtension = ".tmp")
            : base(
                  directory,
                  isTrackingDirty,
                  backupSettings,
                  extension,
                  tempExtension)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySettings"/> class.
        /// Needed for serialization
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private RepositorySettings()
            : base(Directories.AppDirectory(), false, Core.BackupSettings.Create(Directories.AppDirectory()))
        {
        }

        /// <summary>
        /// A default instance for <paramref name="directory"/>
        /// </summary>
        public static RepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new RepositorySettings(directory, false, BackupSettings.Create(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }

        public static RepositorySettings Create(Core.RepositorySettings settings)
        {
            return new RepositorySettings(
                       settings.Directory,
                       settings.IsTrackingDirty,
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
            this.SetPrivate(nameof(this.BackupSettings), reader.ReadElementBackupSettings(nameof(this.BackupSettings)));
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
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
            Ensure.NotNull(field, nameof(field));
            field.SetValue(this, value);
        }
    }
}