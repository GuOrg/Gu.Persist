namespace Gu.Settings.RuntimeBinary
{
    using System;
    using System.IO;

    using Gu.Settings.Core;

    /// <summary>
    /// Specifies the behavior of a <see cref="BinaryRepository"/>
    /// </summary>
    [Serializable]
    public class BinaryRepositorySettings : RepositorySettings
    {
        /// <summary> Initializes a new instance of the <see cref="BinaryRepositorySettings"/> class. </summary>
        public BinaryRepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="BinaryRepositorySettings"/> class. </summary>
        public BinaryRepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="BinaryRepositorySettings"/> class. </summary>
        public BinaryRepositorySettings(DirectoryInfo directory, bool isTrackingDirty, bool isCaching, BackupSettings backupSettings, string extension = ".cfg", string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, isCaching, backupSettings, extension, tempExtension)
        {
        }

        /// <summary>Return the default settings for <paramref name="directory"/> </summary>
        public new static BinaryRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new BinaryRepositorySettings(directory, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}
