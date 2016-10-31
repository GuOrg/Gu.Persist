namespace Gu.Persist.RuntimeXml
{
    using System;
    using System.IO;

    using Gu.Persist.Core;

    /// <summary>
    /// Specifies the behavior of a <see cref="XmlRepository"/>
    /// </summary>
    [Serializable]
    public class XmlRepositorySettings : RepositorySettings
    {
        /// <summary> Initializes a new instance of the <see cref="XmlRepositorySettings"/> class. </summary>
        public XmlRepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="XmlRepositorySettings"/> class. </summary>
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

        /// <summary>
        /// A default instance for <paramref name="directory"/>
        /// </summary>
        public static XmlRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new XmlRepositorySettings(directory, true, false, BackupSettings.Create(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}