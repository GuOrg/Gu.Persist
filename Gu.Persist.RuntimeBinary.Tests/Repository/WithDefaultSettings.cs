namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;
    using NUnit.Framework;

    public class WithDefaultSettings : BinaryRepositoryTests
    {
        [Test]
        public void DefaultSettings()
        {
            Assert.Fail("assert stuff");
        }

        protected override IRepository Create()
        {
            return new SingletonRepository(this.Directory);
        }
    }
}