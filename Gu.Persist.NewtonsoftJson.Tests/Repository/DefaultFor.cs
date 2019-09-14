namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.NewtonsoftJson;
    using NUnit.Framework;

    public class DefaultFor : JsonRepositoryTests
    {
        [Test]
        public void SaveAndCheckJson()
        {
            var fileInfo = this.Directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            var value = new DummySerializable
            {
                Value = 1,
            };
            this.Repository.Save(fileInfo, value);
            var json = System.IO.File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("{\r\n  \"Value\": 1\r\n}", json);
            System.IO.File.Delete(fileInfo.FullName);
        }

        protected override IRepository CreateRepository()
        {
            return new SingletonRepository(this.Directory);
        }
    }
}