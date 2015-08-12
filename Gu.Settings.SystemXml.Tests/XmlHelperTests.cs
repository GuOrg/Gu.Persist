namespace Gu.Settings.SystemXml.Tests
{
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Settings.Core.Tests;

    using NUnit.Framework;

    public class XmlHelperTests
    {
        private FileInfo _file;

        [SetUp]
        public void SetUp()
        {
            _file = new FileInfo(@"C:\Temp\XmlHelperTests.tmp");
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
            if (_file.Exists)
            {
                _file.Delete();
            }
            var dummy = new DummySerializable { Value = 1 };
            XmlHelper.Save(dummy, _file);
            AssertFile.Exists(true, _file);

            var read = XmlHelper.Read<DummySerializable>(_file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task FileAsyncRoundtrip()
        {
            if (_file.Exists)
            {
                _file.Delete();
            }
            var dummy = new DummySerializable { Value = 1 };
            await XmlHelper.SaveAsync(dummy, _file).ConfigureAwait(false);
            AssertFile.Exists(true, _file);

            var read = await XmlHelper.ReadAsync<DummySerializable>(_file).ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }
    }
}