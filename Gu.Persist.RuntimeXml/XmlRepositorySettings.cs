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

        /// <summary> Initializes a new instance of the <see cref="XmlRepositorySettings"/> class. </summary>
        public XmlRepositorySettings(DirectoryInfo directory, bool isTrackingDirty, bool isCaching, BackupSettings backupSettings, string extension = ".cfg", string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, isCaching, backupSettings, extension, tempExtension)
        {
        }

        /// <summary>
        /// A default instance for <paramref name="directory"/>
        /// </summary>
        public new static XmlRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new XmlRepositorySettings(directory, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}