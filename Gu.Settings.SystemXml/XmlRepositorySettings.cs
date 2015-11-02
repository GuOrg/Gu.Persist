namespace Gu.Settings.SystemXml
{
    using System.IO;
    using Gu.Settings.Core;

    public class XmlRepositorySettings : RepositorySettings
    {
        private XmlRepositorySettings()
        {
        }

        public XmlRepositorySettings(DirectoryInfo directory)
            : base(directory)
        {
        }

        public XmlRepositorySettings(DirectoryInfo directory, BackupSettings backupSettings)
            : base(directory, backupSettings)
        {
        }

        public XmlRepositorySettings(DirectoryInfo directory, bool isTrackingDirty, bool isCaching, BackupSettings backupSettings, string extension = ".cfg", string tempExtension = ".tmp")
            : base(directory, isTrackingDirty, isCaching, backupSettings, extension, tempExtension)
        {
        }

        public static XmlRepositorySettings DefaultFor(DirectoryInfo directory)
        {
            return new XmlRepositorySettings(directory, true, true, BackupSettings.DefaultFor(directory.CreateSubdirectory(DefaultBackupDirectoryName)));
        }
    }
}