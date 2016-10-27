namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.Core.Tests.Repositories;
    using Gu.Persist.NewtonsoftJson;
    using NUnit.Framework;

    public class JsonNullSettings : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, this.RepoSettingFile);
        }

        protected override IRepository Create()
        {
            return new JsonRepository();
        }

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
