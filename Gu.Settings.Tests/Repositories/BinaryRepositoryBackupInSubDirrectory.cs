namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    public class BinaryRepositoryBackupInSubDirrectory : RepositoryTests
    {
        protected override RepositorySettings Settings
        {
            get { return RepositorySettings.DefaultFor(Directory); }
        }

        protected override BackupSettings BackupSettings
        {
            get { return new BackupSettings(Directory.Subdirectory("Backup"), 1); }
        }

        protected override IRepository Create()
        {
            return new Settings.BinaryRepository(Settings);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            BinaryHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return BinaryHelper.Read<T>(file);
        }
    }
}