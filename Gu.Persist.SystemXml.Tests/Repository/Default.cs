namespace Gu.Persist.SystemXml.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.SystemXml;
    using NUnit.Framework;

    public class Default : XmlRepositoryTests
    {
        [Test]
        public void CreateSecond()
        {
            Assert.DoesNotThrow(() => this.Create());
        }

        protected override IRepository Create()
        {
            return new XmlRepository();
        }
    }
}