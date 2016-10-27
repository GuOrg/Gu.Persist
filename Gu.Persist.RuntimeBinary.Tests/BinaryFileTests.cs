namespace Gu.Persist.RuntimeBinary.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.RuntimeBinary;
    using NUnit.Framework;

    public class BinaryFileTests
    {
        private readonly DirectoryInfo directory;

        public BinaryFileTests()
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
            var clone = BinaryFile.Clone(dummy);
            Assert.AreNotSame(dummy, clone);
            Assert.AreEqual(dummy.Value, clone.Value);
        }

        [Test]
        public void SaveThenRead()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            BinaryFile.Save(file, dummy);
            var read = BinaryFile.Read<DummySerializable>(file);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveThenReadAsync()
        {
            var dummy = new DummySerializable { Value = 1 };
            var file = this.directory.CreateFileInfoInDirectory("dummy.json");
            await BinaryFile.SaveAsync(file, dummy).ConfigureAwait(false);
            var read = await BinaryFile.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreNotSame(dummy, read);
            Assert.AreEqual(dummy.Value, read.Value);
        }
    }
}
