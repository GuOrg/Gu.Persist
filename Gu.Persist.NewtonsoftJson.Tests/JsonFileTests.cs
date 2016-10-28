namespace Gu.Persist.NewtonsoftJson.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Newtonsoft.Json;
    using NUnit.Framework;

    public class JsonFileTests
    {
        private static readonly JsonSerializerSettings JsonSettings = JsonRepositorySettings.CreateDefaultJsonSettings();
        private readonly DirectoryInfo directory;

        public JsonFileTests()
        {
            this.directory = new DirectoryInfo($@"C:\Temp\Gu.Persist\{this.GetType().Name}").CreateIfNotExists();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.directory.Delete(true);
        }

        [Test]
        public void Clone()
        {
            var dummy = new DummySerializable { Value = 1 };
            var clone = JsonFile.Clone(dummy);
            Assert.AreNotSame(dummy, clone);
            Assert.AreEqual(dummy.Value, clone.Value);
        }

        [Test]
        public void CloneWithSettings()
        {
            var dummy = new DummySerializable { Value = 1 };
            var clone = JsonFile.Clone(dummy, JsonSettings);
            Assert.AreNotSame(dummy, clone);
            Assert.AreEqual(dummy.Value, clone.Value);
        }

        [Test]
        public void SaveTwice()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            JsonFile.Save(file, dummy);
            JsonFile.Save(file, dummy);
            var read = JsonFile.Read<DummySerializable>(file);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public void SaveThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            JsonFile.Save(file, dummy);
            var read = JsonFile.Read<DummySerializable>(file);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public void SaveThenReadWithSettings()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            JsonFile.Save(file, dummy, JsonSettings);
            var read = JsonFile.Read<DummySerializable>(file, JsonSettings);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public void SaveTwiceWithSettings()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            JsonFile.Save(file, dummy, JsonSettings);
            JsonFile.Save(file, dummy, JsonSettings);
            var read = JsonFile.Read<DummySerializable>(file, JsonSettings);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveThenReadAsync()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            await JsonFile.SaveAsync(file, dummy).ConfigureAwait(false);
            var read = await JsonFile.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncTwice()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            await JsonFile.SaveAsync(file, dummy).ConfigureAwait(false);
            await JsonFile.SaveAsync(file, dummy).ConfigureAwait(false);
            var read = await JsonFile.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveThenReadAsyncWithSettings()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            await JsonFile.SaveAsync(file, dummy, JsonSettings).ConfigureAwait(false);
            var read = await JsonFile.ReadAsync<DummySerializable>(file, JsonSettings).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncTwiceWithSettings()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            await JsonFile.SaveAsync(file, dummy, JsonSettings).ConfigureAwait(false);
            await JsonFile.SaveAsync(file, dummy, JsonSettings).ConfigureAwait(false);
            var read = await JsonFile.ReadAsync<DummySerializable>(file, JsonSettings).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }
    }
}