namespace Gu.Settings
{
    public interface IRepositorySettings : IFileSettings
    {
        string DirectoryPath { get;  }
        BackupSettings BackupSettings { get;  }
        string TempExtension { get;  }
        bool IsTrackingDirty { get;  }
    }
}