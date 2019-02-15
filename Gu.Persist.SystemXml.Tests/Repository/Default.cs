namespace Gu.Persist.SystemXml.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.SystemXml;
    using NUnit.Framework;

#pragma warning disable CA1716 // Identifiers should not match keywords
    public class Default : XmlRepositoryTests
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        [Test]
        public void CreateSecond()
        {
            Assert.DoesNotThrow(() => this.Create());
        }

        protected override IRepository Create()
        {
            return new SingletonRepository();
        }
    }
}