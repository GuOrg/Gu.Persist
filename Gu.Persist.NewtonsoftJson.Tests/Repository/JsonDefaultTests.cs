namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.Core.Tests.Repositories;
    using Gu.Persist.NewtonsoftJson;
    using NUnit.Framework;

    public class JsonDefaultTests : RepositoryTests
    {
        [Test]
        public void SaveAndCheckJson()
        {
            var fileInfo = this.Directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            var value = new DummySerializable
            {
                Value = 1
            };
            this.Repository.Save(value, fileInfo);
            var json = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("{\r\n  \"Value\": 1\r\n}", json);
            File.Delete(fileInfo.FullName);
        }

        protected override IRepository Create()
        {
            var settings = JsonRepositorySettings.DefaultFor(this.Directory);
            return new JsonRepository(settings);
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