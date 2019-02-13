namespace Gu.Persist.RuntimeBinary.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;

    using NUnit.Framework;

    public class BinaryFileTests
    {
        private readonly DirectoryInfo directory;

        public BinaryFileTests()
        {
            this.directory = new DirectoryInfo($@"C:\Temp\Gu.Persist\{this.GetType().Name}").CreateIfNotExists();
        }

        [SetUp]
        public void SetUp()
        {
            this.directory.Delete(recursive: true);
            this.directory.Create();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.directory.Delete(recursive: true);
        }

        [Test]
        public void Clone()
        {
            var dummy = new DummySerializable { Value = 1 };
            var clone = BinaryFile.Clone(dummy);
            Assert.AreNotSame(dummy, clone);
            Assert.AreEqual(dummy.Value, clone.Value);
        }

        [Test]
        public void SaveThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.binary");
            BinaryFile.Save(file, dummy);
            var read = BinaryFile.Read<DummySerializable>(file);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public void SaveThenReadName()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.binary");
            BinaryFile.Save(file.FullName, dummy);
            var read = BinaryFile.Read<DummySerializable>(file.FullName);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public void SaveTwiceThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.binary");
            BinaryFile.Save(file, dummy);
            BinaryFile.Save(file, dummy);
            var read = BinaryFile.Read<DummySerializable>(file);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.binary");
            await BinaryFile.SaveAsync(file, dummy).ConfigureAwait(false);
            var read = await BinaryFile.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncThenReadName()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.binary");
            await BinaryFile.SaveAsync(file.FullName, dummy).ConfigureAwait(false);
            var read = await BinaryFile.ReadAsync<DummySerializable>(file.FullName).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveAsyncTwiceThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.binary");
            await BinaryFile.SaveAsync(file, dummy).ConfigureAwait(false);
            await BinaryFile.SaveAsync(file, dummy).ConfigureAwait(false);
            var read = await BinaryFile.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }
    }
}
