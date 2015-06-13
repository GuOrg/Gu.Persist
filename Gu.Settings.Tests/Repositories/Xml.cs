namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    public class Xml : RepositoryTests
    {
        protected override IRepository Create(RepositorySetting setting)
        {
            return new XmlRepository(setting);
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