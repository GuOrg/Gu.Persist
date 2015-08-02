namespace Gu.Settings.Tests.Repositories
{
    using System.IO;
    using NUnit.Framework;

    public class BinaryRepositoryNullSettings : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, RepoSettingFile);
        }


        protected override IRepository Create()
        {
            return new Settings.BinaryRepository();
        }

        protected override RepositorySettings Settings
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }
                return (RepositorySettings)Repository.Settings;
            }
        }

        protected override BackupSettings BackupSettings
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }
                return Repository.Settings.BackupSettings;
            }
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