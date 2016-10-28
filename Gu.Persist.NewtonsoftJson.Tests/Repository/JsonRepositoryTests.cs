namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core.Tests.Repositories;

    public abstract class JsonRepositoryTests : RepositoryTests
    {
        protected override void Save<T>(T item, FileInfo file)
        {
            JsonFile.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return JsonFile.Read<T>(file);
        }
    }
}