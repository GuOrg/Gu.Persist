namespace Gu.Settings.Tests.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Gu.Settings.Tests.Helpers;

    using NUnit.Framework;

    public class BinaryHelperTests
    {
        private FileInfo _file;

        [SetUp]
        public void SetUp()
        {
            _file = new FileInfo(@"C:\Temp\BinaryHelperTests.tmp");
            _file.Delete();
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
            BinaryHelper.Save(dummy, _file);
            AssertFile.Exists(true, _file);

            var read = BinaryHelper.Read<DummySerializable>(_file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task FileAsyncRoundtrip()
        {
            var dummy = new DummySerializable { Value = 1 };
            await BinaryHelper.SaveAsync(dummy, _file);
            AssertFile.Exists(true, _file);

            var read = await BinaryHelper.ReadAsync<DummySerializable>(_file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }
    }
}
