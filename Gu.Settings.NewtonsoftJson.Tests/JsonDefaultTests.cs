namespace Gu.Settings.NewtonsoftJson.Tests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests.Repositories;
    using Gu.Settings.NewtonsoftJson;

    public class JsonDefaultTests : RepositoryTests
    {
        protected override IRepository Create()
        {
            var settings = JsonRepositorySettings.DefaultFor(Directory);
            return new JsonRepository(settings);
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