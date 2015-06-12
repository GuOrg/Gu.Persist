namespace Gu.Settings.Tests.IO
{
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Settings.Tests.Helpers;

    using NUnit.Framework;

    public class XmlRepositoryTests
    {
        private FileInfo _file;
        private FileInfo _tempFile;
        private FileInfo _backup;
        private FileInfo _dummyFile;

        private RepositorySetting _setting;
        private DummySerializable _dummy;
        private XmlRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _file = new FileInfo(string.Format(@"C:\Temp\{0}.cfg", GetType().Name));
            _tempFile = new FileInfo(string.Format(@"C:\Temp\{0}.tmp", GetType().Name));
            _backup = new FileInfo(string.Format(@"C:\Temp\{0}.bak", GetType().Name));
            _setting = new RepositorySetting(true, true, _file.Directory, ".cfg", ".tmp", ".bak");
            _dummyFile = new FileInfo(string.Format(@"C:\Temp\{0}.cfg", typeof(DummySerializable).Name));
            _file.Delete();
            _tempFile.Delete();
            _backup.Delete();
            _dummyFile.Delete();
            _dummy = new DummySerializable(1);
            _repository = new XmlRepository(_setting);
        }

        [Test]
        public void Read()
        {
            Save(_dummy, _file);
            var read = _repository.Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void ReadAuto()
        {
            Save(_dummy, _dummyFile);
            var read = _repository.Read<DummySerializable>();
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void ReadCaches()
        {
            Save(_dummy, _file);
            var read1 = _repository.Read<DummySerializable>(_file);
            var read2 = _repository.Read<DummySerializable>(_file);
            Assert.AreSame(read1, read2);
        }

        [Test]
        public void Save()
        {
            _repository.Save(_dummy, _file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public async Task SaveAsync()
        {
            await _repository.SaveAsync(_dummy, _file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void SaveCreatesBackup()
        {
            _repository.Save(_dummy, _file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);

            _dummy.Value = 2;
            _repository.Save(_dummy, _file);

            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void SaveAuto()
        {
            _repository.Save(_dummy);
            AssertFile.Exists(true, _dummyFile);
        }

        [Test]
        public void SaveCaches()
        {
            _repository.Save(_dummy, _file);
            var read = _repository.Read<DummySerializable>(_file);
            Assert.AreSame(_dummy, read);
        }

        [Test]
        public void SaveThreeTimes()
        {
            _repository.Save(_dummy, _file);
            var read = _repository.Read<DummySerializable>(_file);
            Assert.AreSame(_dummy, read);
            read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy, read);

            for (int i = 2; i < 3; i++)
            {
                _dummy.Value++;
                _repository.Save(_dummy, _file);
                read = _repository.Read<DummySerializable>(_file);
                Assert.AreSame(_dummy, read);
                read = Read<DummySerializable>(_file);
                Assert.AreEqual(_dummy, read);

                read = Read<DummySerializable>(_backup);
                Assert.AreEqual(_dummy.Value - 1, read.Value);
            }
        }

        [Test]
        public void IsDirty()
        {
            Assert.IsTrue(_repository.IsDirty(_dummy));

            _repository.Save(_dummy);
            Assert.IsFalse(_repository.IsDirty(_dummy));

            _dummy.Value++;
            Assert.IsTrue(_repository.IsDirty(_dummy));
        }

        private static void Save<T>(T item, FileInfo file)
        {
            XmlHelper.Save(item, file);
        }

        private static T Read<T>(FileInfo file)
        {
            return XmlHelper.Read<T>(file);
        }
    }
}
