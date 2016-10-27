namespace Gu.Persist.RuntimeXml.Tests
{
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests.Repositories;

    public class XmlRepositoryTests : RepositoryTests
    {
        protected override IRepository Create()
        {
            return new XmlRepository();
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            XmlTestHelper.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return XmlTestHelper.Read<T>(file);
        }
    }
}