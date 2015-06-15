namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    using NUnit.Framework;

    public class BinaryDefault : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, RepoSettingFile);
        }

        [Test]
        public void DefaultSettings()
        {
            Assert.AreEqual(RepositorySettings.DefaultFor(Directory), Repository.Settings);
        }

        protected override IRepository Create(RepositorySettings settings)
        {
            return new BinaryRepository();
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