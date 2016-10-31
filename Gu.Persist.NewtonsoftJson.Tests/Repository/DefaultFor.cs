namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.NewtonsoftJson;
    using NUnit.Framework;

    using File = System.IO.File;
    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class DefaultFor : JsonRepositoryTests
    {
        [Test]
        public void SaveAndCheckJson()
        {
            var fileInfo = this.Directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            var value = new DummySerializable
            {
                Value = 1
            };
            this.Repository.Save(fileInfo, value);
            var json = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("{\r\n  \"Value\": 1\r\n}", json);
            File.Delete(fileInfo.FullName);
        }

        protected override IRepository Create()
        {
            var settings = Core.RepositorySettings.DefaultFor(this.Directory);
            return new SingletonRepository(settings);
        }
    }
}