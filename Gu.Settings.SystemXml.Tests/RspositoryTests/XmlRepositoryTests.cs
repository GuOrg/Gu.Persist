namespace Gu.Settings.SystemXml.Tests.RspositoryTests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests.Repositories;
    using Gu.Settings.SystemXml;
    using Gu.Settings.SystemXml.Tests.Helpers;

    public class XmlRepositoryTests : RepositoryTests
    {
        protected override IRepository Create()
        {
            return new XmlRepository();
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            TestHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return TestHelper.Read<T>(file);
        }
    }
}