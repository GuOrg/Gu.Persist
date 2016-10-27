namespace Gu.Persist.RuntimeBinary.Tests
{
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests.Repositories;

    using NUnit.Framework;

    public class BinaryRepositoryDefaultSettings : RepositoryTests
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
            BinaryHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return BinaryHelper.Read<T>(file);
        }
    }
}