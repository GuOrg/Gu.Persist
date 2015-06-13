namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    using NUnit.Framework;

    public class XmlDefault : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, RepoSettingFile);
        }

        protected override IRepository Create(RepositorySetting setting)
        {
            return new XmlRepository(Directory);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            XmlHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return XmlHelper.Read<T>(file);
        }
    }
}