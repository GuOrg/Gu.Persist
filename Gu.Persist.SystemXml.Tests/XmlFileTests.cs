namespace Gu.Persist.SystemXml.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using NUnit.Framework;

    public class XmlFileTests
    {
        private readonly DirectoryInfo directory;

        public XmlFileTests()
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
            var clone = XmlFile.Clone(dummy);
            Assert.AreNotSame(dummy, clone);
            Assert.AreEqual(dummy.Value, clone.Value);
        }

        [Test]
        public void SaveThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.xml");
            XmlFile.Save(file, dummy);
            var read = XmlFile.Read<DummySerializable>(file);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public void SaveThenReadName()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.xml");
            XmlFile.Save(file.Name, dummy);
            var read = XmlFile.Read<DummySerializable>(file.Name);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public void SaveTwiceThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.xml");
            XmlFile.Save(file, dummy);
            XmlFile.Save(file, dummy);
            var read = XmlFile.Read<DummySerializable>(file);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.xml");
            await XmlFile.SaveAsync(file, dummy).ConfigureAwait(false);
            var read = await XmlFile.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncThenReadName()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.xml");
            await XmlFile.SaveAsync(file.Name, dummy).ConfigureAwait(false);
            var read = await XmlFile.ReadAsync<DummySerializable>(file.Name).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncTwiceThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.xml");
            await XmlFile.SaveAsync(file, dummy).ConfigureAwait(false);
            await XmlFile.SaveAsync(file, dummy).ConfigureAwait(false);
            var read = await XmlFile.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }
    }
}
