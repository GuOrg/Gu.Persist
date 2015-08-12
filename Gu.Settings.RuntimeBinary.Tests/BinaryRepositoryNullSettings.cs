namespace Gu.Settings.RuntimeBinary.Tests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests;
    using Gu.Settings.Core.Tests.Repositories;

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
            return new BinaryRepository();
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