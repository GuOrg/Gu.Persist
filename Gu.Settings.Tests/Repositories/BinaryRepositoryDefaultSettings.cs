namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    using NUnit.Framework;

    public class BinaryRepositoryDefaultSettings : RepositoryTests
    {
        [Test]
        public void DefaultSettings()
        {
            var defaultSettings = RepositorySettings.DefaultFor(Directory);
            var comparer = new BinaryEqualsComparer<IRepositorySettings>();
            Assert.IsTrue(comparer.Equals(defaultSettings, Repository.Settings));
        }

        protected override IRepository Create()
        {
            return new Settings.BinaryRepository(Settings);
        }

        protected override RepositorySettings Settings
        {
            get { return RepositorySettings.DefaultFor(Directory); }
        }

        protected override BackupSettings BackupSettings
        {
            get { return BackupSettings.DefaultFor(Directory); }
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