namespace Gu.Settings
{
    using System.IO;

    public interface IRepositorySetting
    {
        bool CreateBackupOnSave { get; }
        DirectoryInfo Directory { get; }
        string Extension { get; }
        string BackupExtension { get; }
        bool IsTrackingDirty { get; }
    }
}