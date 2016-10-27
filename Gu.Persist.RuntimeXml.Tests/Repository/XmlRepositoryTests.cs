namespace Gu.Persist.RuntimeXml.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core.Tests.Repositories;

    public abstract class XmlRepositoryTests : RepositoryTests
    {
        protected override void Save<T>(T item, FileInfo file)
        {
            XmlFile.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return XmlFile.Read<T>(file);
        }
    }
}