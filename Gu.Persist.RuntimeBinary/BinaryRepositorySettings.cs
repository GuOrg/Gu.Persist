namespace Gu.Persist.RuntimeBinary
{
    using System;
    using System.IO;

    using Gu.Persist.Core;

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
        public BinaryRepositorySettings(
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

        /// <summary>Return the default settings for <paramref name="directory"/> </summary>
        public static BinaryRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new BinaryRepositorySettings(
                directory,
                true,
                true,
                false,
                BackupSettings.Create(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}
