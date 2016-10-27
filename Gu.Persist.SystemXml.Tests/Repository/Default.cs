namespace Gu.Persist.SystemXml.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.SystemXml;

    public class Default : XmlRepositoryTests
    {
        protected override IRepository Create()
        {
            return new XmlRepository();
        }
    }
}