namespace Gu.Persist.SystemXml.Tests
{
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Persist.Core.Tests;

    using NUnit.Framework;

    public class XmlHelperTests
    {
        private FileInfo file;

        [SetUp]
        public void SetUp()
        {
            this.file = new FileInfo(@"C:\Temp\XmlHelperTests.tmp");
        }

        [Test]
        public void DeepClone()
        {
            var dummy = new DummySerializable { Value = 1 };
            var clone = XmlHelper.Clone(dummy);
            Assert.AreEqual(dummy.Value, clone.Value);
            Assert.AreNotSame(dummy, clone);
        }

        [Test]
        public void FileRoundtrip()
        {
            if (this.file.Exists)
            {
                this.file.Delete();
            }

            var dummy = new DummySerializable { Value = 1 };
            XmlHelper.Save(dummy, this.file);
            AssertFile.Exists(true, this.file);

            var read = XmlHelper.Read<DummySerializable>(this.file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task FileAsyncRoundtrip()
        {
            if (this.file.Exists)
            {
                this.file.Delete();
            }

            var dummy = new DummySerializable { Value = 1 };
            await XmlHelper.SaveAsync(dummy, this.file).ConfigureAwait(false);
            AssertFile.Exists(true, this.file);

            var read = await XmlHelper.ReadAsync<DummySerializable>(this.file).ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }
    }
}