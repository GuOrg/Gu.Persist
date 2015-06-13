namespace Gu.Settings.Json.Tests
{
    using System.IO;

    using Gu.Settings.Tests.Repositories;

    public class Json : RepositoryTests
    {
        protected override IRepository Create(RepositorySetting setting)
        {
            return new JsonRepository(setting);
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
