namespace Gu.Settings.RuntimeXml
{
    using System;
    using System.IO;

    using Gu.Settings.Core;

    [Serializable]
    public class RuntimeXmlRepositorySettings : RepositorySettings
    {
        public RuntimeXmlRepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
        }

        public RuntimeXmlRepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
        }

        public RuntimeXmlRepositorySettings(DirectoryInfo directory, bool isTrackingDirty, bool isCaching, BackupSettings backupSettings, string extension = ".cfg", string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, isCaching, backupSettings, extension, tempExtension)
        {
        }

        public new static RuntimeXmlRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new RuntimeXmlRepositorySettings(directory, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}