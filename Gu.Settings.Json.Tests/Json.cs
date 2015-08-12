namespace Gu.Settings.Json.Tests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests;
    using Gu.Settings.Core.Tests.Repositories;

    using NUnit.Framework;

    public class JsonNullSettings : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, RepoSettingFile);
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

        protected override IRepository Create()
        {
            return new JsonRepository();
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            TestHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return TestHelper.Read<T>(file);
        }
    }
}
