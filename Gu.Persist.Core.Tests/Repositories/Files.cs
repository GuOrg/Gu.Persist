namespace Gu.Persist.Core.Tests.Repositories
{
    using System.IO;

    using Gu.Persist.Core.Backup;

    public class Files
    {
        public const string NewName = "NewName";
        public readonly FileInfo File;
        public readonly FileInfo Temp;
        public readonly FileInfo SoftDelete;
        public readonly FileInfo SoftDeleteNewName;
        public readonly FileInfo WithNewName;

        public readonly FileInfo Backup;
        public readonly FileInfo BackupSoftDelete;
        public readonly FileInfo BackupNewName;

        public Files(string name, RepositorySettings settings)
        {
            var fileName = string.Concat(name, settings.Extension);
            var settingsDirectory = new DirectoryInfo(settings.Directory);
            this.File = settingsDirectory.CreateFileInfoInDirectory(fileName);

            var tempfileName = string.Concat(name, settings.TempExtension);
            this.Temp = settingsDirectory.CreateFileInfoInDirectory(tempfileName);
            this.SoftDelete = this.File.GetSoftDeleteFileFor();
            this.WithNewName = this.File.WithNewName(NewName, settings);
            this.SoftDeleteNewName = this.SoftDelete.WithNewName(NewName, settings);

            var backupSettings = settings.BackupSettings;
            if (backupSettings != null)
            {
                this.Backup = BackupFile.CreateFor(this.File, backupSettings);
                this.BackupSoftDelete = this.Backup.GetSoftDeleteFileFor();
                this.BackupNewName = BackupFile.CreateFor(this.WithNewName, backupSettings);
            }
        }
    }
}