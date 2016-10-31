namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;
    using NUnit.Framework;

    public class WithDefaultSettings : BinaryRepositoryTests
    {
        [Test]
        public void DefaultSettings()
        {
            var defaultSettings = RepositorySettings.DefaultFor(this.Directory);
            var comparer = new BinaryEqualsComparer<IRepositorySettings>();
            Assert.IsTrue(comparer.Equals(defaultSettings, this.Repository.Settings));
        }

        protected override IRepository Create()
        {
            var settings = RepositorySettings.DefaultFor(this.Directory);
            return new SingletonRepository(settings);
        }
    }
}