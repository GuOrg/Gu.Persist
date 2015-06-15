namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    public class Xml : RepositoryTests
    {
        protected override IRepository Create(RepositorySettings settings)
        {
            return new XmlRepository(settings);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            XmlHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return XmlHelper.Read<T>(file);
        }
    }
}