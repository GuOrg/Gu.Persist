namespace Gu.Persist.Yaml.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core.Tests.Repositories;

    public abstract class YamlRepositoryTests : RepositoryTests
    {
        protected override void Save<T>(FileInfo file, T item)
        {
            YamlFile.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return YamlFile.Read<T>(file);
        }
    }
}