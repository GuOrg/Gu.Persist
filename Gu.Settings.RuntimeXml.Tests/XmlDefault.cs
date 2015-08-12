namespace Gu.Settings.RuntimeXml.Tests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests;
    using Gu.Settings.Core.Tests.Repositories;
    using Gu.Settings.RuntimeXml;

    using NUnit.Framework;

    public class XmlDefault : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, RepoSettingFile);
        }

        protected override RepositorySettings Settings
        {
            get { return RepositorySettings.DefaultFor(Directory);}
        }

        protected override BackupSettings BackupSettings
        {
            get { return BackupSettings.DefaultFor(Directory);}
        }

        protected override IRepository Create()
        {
            return new XmlRepository();
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