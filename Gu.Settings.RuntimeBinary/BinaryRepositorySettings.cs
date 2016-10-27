namespace Gu.Settings.RuntimeBinary
{
    using System;
    using System.IO;

    using Gu.Settings.Core;

    [Serializable]
    public class BinaryRepositorySettings : RepositorySettings
    {
        public BinaryRepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
        }

        public BinaryRepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
        }

        public BinaryRepositorySettings(DirectoryInfo directory, bool isTrackingDirty, bool isCaching, BackupSettings backupSettings, string extension = ".cfg", string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, isCaching, backupSettings, extension, tempExtension)
        {
        }

        public new static BinaryRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new BinaryRepositorySettings(directory, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}
