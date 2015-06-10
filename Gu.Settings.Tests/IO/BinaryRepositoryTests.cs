namespace Gu.Settings.Tests.IO
{
    using System.IO;

    using Gu.Settings.Tests.Helpers;

    using NUnit.Framework;

    public class BinaryRepositoryTests
    {
        private FileInfo _file;
        private FileInfo _backup;
        private RepositorySetting _setting;
        private FileInfo _autoFile;

        [SetUp]
        public void SetUp()
        {
            _file = new FileInfo(@"C:\Temp\BinaryRepositoryTests.tmp");
            _backup = new FileInfo(@"C:\Temp\BinaryRepositoryTests.bak");
            _setting = new RepositorySetting(true, _file.Directory, ".tmp", ".bak");
            _autoFile = new FileInfo(@"C:\Temp\DummySerializable.tmp");
            _file.Delete();
            _backup.Delete();
            _autoFile.Delete();
        }

        [Test]
        public void Read()
        {
            var dummy = new DummySerializable(1);
            BinaryHelper.Save(dummy, _file);
            var repository = new BinaryRepository(_setting);
            var read = repository.Read<DummySerializable>(_file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void ReadAuto()
        {
            var dummy = new DummySerializable(1);
            BinaryHelper.Save(dummy, _autoFile);
            var repository = new BinaryRepository(_setting);
            var read = repository.Read<DummySerializable>();
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void ReadCaches()
        {
            var dummy = new DummySerializable(1);
            BinaryHelper.Save(dummy, _file);
            var repository = new BinaryRepository(_setting);
            var read1 = repository.Read<DummySerializable>(_file);
            var read2 = repository.Read<DummySerializable>(_file);
            Assert.AreSame(read1, read2);
        }

        [Test]
        public void Save()
        {
            var dummy = new DummySerializable(1);
            var repository = new BinaryRepository(_setting);
            repository.Save(dummy, _file);
            AssertExists(true, _file);
            AssertExists(false, _backup);
            var read = BinaryHelper.Read<DummySerializable>(_file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void SaveCreatesBackup()
        {
            var dummy = new DummySerializable(1);
            var repository = new BinaryRepository(_setting);

            repository.Save(dummy, _file);
            AssertExists(true, _file);
            AssertExists(false, _backup);

            dummy.Value = 2;
            repository.Save(dummy, _file);

            AssertExists(true, _file);
            AssertExists(true, _backup);
            var read = BinaryHelper.Read<DummySerializable>(_file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void SaveAuto()
        {
            var dummy = new DummySerializable(1);
            var repository = new BinaryRepository(_setting);
            repository.Save(dummy);
            Assert.IsTrue(_autoFile.Exists);
        }

        [Test]
        public void SaveCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = new BinaryRepository(_setting);
            repository.Save(dummy, _file);
            var read = repository.Read<DummySerializable>(_file);
            Assert.AreSame(dummy, read);
        }

        public static void AssertExists(bool expected, FileInfo fileInfo)
        {
            fileInfo.Refresh();
            Assert.AreEqual(expected, fileInfo.Exists);
        }
    }
}