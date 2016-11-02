namespace Gu.Persist.Yaml.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Core;
    using Gu.Persist.Core.Tests;
    using NUnit.Framework;

    public class YamlFileTests
    {
        private readonly DirectoryInfo directory;

        public YamlFileTests()
        {
            this.directory = new DirectoryInfo($@"C:\Temp\Gu.Persist\{this.GetType().Name}").CreateIfNotExists();
        }

        [SetUp]
        public void SetUp()
        {
            this.directory.Delete(true);
            this.directory.Create();
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
            var clone = YamlFile.Clone(dummy);
            Assert.AreNotSame(dummy, clone);
            Assert.AreEqual(dummy.Value, clone.Value);
        }

        [Test]
        public void SaveThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.yml");
            YamlFile.Save(file, dummy);
            var read = YamlFile.Read<DummySerializable>(file);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public void SaveThenReadName()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.yml");
            YamlFile.Save(file.Name, dummy);
            var read = YamlFile.Read<DummySerializable>(file.Name);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public void SaveTwiceThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.yml");
            YamlFile.Save(file, dummy);
            YamlFile.Save(file, dummy);
            var read = YamlFile.Read<DummySerializable>(file);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.yml");
            await YamlFile.SaveAsync(file, dummy).ConfigureAwait(false);
            var read = await YamlFile.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncThenReadName()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.yml");
            await YamlFile.SaveAsync(file.Name, dummy).ConfigureAwait(false);
            var read = await YamlFile.ReadAsync<DummySerializable>(file.Name).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncTwiceThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.yml");
            await YamlFile.SaveAsync(file, dummy).ConfigureAwait(false);
            await YamlFile.SaveAsync(file, dummy).ConfigureAwait(false);
            var read = await YamlFile.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }
    }
}
