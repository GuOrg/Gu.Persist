namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests.Repositories;
    using NUnit.Framework;

    public class WithDefaultSettings : RepositoryTests
    {
        [Test]
        public void DefaultSettings()
        {
            var defaultSettings = BinaryRepositorySettings.DefaultFor(this.Directory);
            var comparer = new BinaryEqualsComparer<IRepositorySettings>();
            Assert.IsTrue(comparer.Equals(defaultSettings, this.Repository.Settings));
        }

        protected override IRepository Create()
        {
            var settings = BinaryRepositorySettings.DefaultFor(this.Directory);
            return new BinaryRepository(settings);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            BinaryFile.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return BinaryFile.Read<T>(file);
        }
    }
}