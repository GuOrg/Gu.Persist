namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core.Tests.Repositories;

    using File = Gu.Persist.NewtonsoftJson.File;

    public abstract class JsonRepositoryTests : RepositoryTests
    {
        protected override void Save<T>(FileInfo file, T item)
        {
            File.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return File.Read<T>(file);
        }
    }
}