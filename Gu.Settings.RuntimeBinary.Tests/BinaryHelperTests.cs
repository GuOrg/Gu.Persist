namespace Gu.Settings.Core.Tests.IO
{
    using System.IO;
    using System.Threading.Tasks;
    using Gu.Settings.RuntimeBinary;

    using NUnit.Framework;

    public class BinaryHelperTests
    {
        private FileInfo file;

        [SetUp]
        public void SetUp()
        {
            this.file = new FileInfo(@"C:\Temp\BinaryHelperTests.tmp");
            this.file.Delete();
        }

        [Test]
        public void DeepClone()
        {
            var dummy = new DummySerializable { Value = 1 };
            var clone = BinaryHelper.Clone(dummy);
            Assert.AreEqual(dummy.Value, clone.Value);
            Assert.AreNotSame(dummy, clone);
        }

        [Test]
        public void FileRoundtrip()
        {
            var dummy = new DummySerializable { Value = 1 };
            BinaryHelper.Save(dummy, this.file);
            AssertFile.Exists(true, this.file);

            var read = BinaryHelper.Read<DummySerializable>(this.file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task FileAsyncRoundtrip()
        {
            var dummy = new DummySerializable { Value = 1 };
            await BinaryHelper.SaveAsync(dummy, this.file).ConfigureAwait(false);
            AssertFile.Exists(true, this.file);

            var read = await BinaryHelper.ReadAsync<DummySerializable>(this.file).ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }
    }
}
