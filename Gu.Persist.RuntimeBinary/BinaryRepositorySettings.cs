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

        /// <summary> Initializes a new instance of the <see cref="BinaryRepositorySettings"/> class. </summary>
        public BinaryRepositorySettings(
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

        /// <summary>Return the default settings for <paramref name="directory"/> </summary>
        public static BinaryRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new BinaryRepositorySettings(
                directory,
                true,
                false,
                BackupSettings.Create(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }

        public static BinaryRepositorySettings Create(RepositorySettings settings)
        {
            return new BinaryRepositorySettings(
                       settings.Directory,
                       settings.IsTrackingDirty,
                       settings.SaveNullDeletesFile,
                       settings.BackupSettings,
                       settings.Extension,
                       settings.TempExtension);
        }
    }
}
