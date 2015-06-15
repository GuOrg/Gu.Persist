namespace Gu.Settings.Json.Tests
{
    using System.IO;

    using Gu.Settings.Tests.Repositories;

    public class Json : RepositoryTests
    {
        protected override IRepository Create(RepositorySettings settings)
        {
            return new JsonRepository(settings);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            JsonHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return JsonHelper.Read<T>(file);
        }
    }
}
