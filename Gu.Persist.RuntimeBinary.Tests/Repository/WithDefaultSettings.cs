namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;
    using NUnit.Framework;

    public class WithDefaultSettings : BinaryRepositoryTests
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
    }
}