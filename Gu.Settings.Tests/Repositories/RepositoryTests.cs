namespace Gu.Settings.Tests.Repositories
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Settings.Tests.Helpers;

    using NUnit.Framework;

    public abstract class RepositoryTests
    {
        public static readonly DirectoryInfo Directory = new DirectoryInfo(@"C:\Temp\Gu.Settings");
        private readonly FileInfo _file;
        private readonly FileInfo _tempFile;
        private readonly FileInfo _backup;
        protected readonly FileInfo RepoSettingFile;

        private readonly FileInfo _dummyFile;
        private readonly FileInfo _dummyBackup;

        private readonly BackupSettings _backupSettings;
        private readonly RepositorySetting _setting;
        private IRepository _repository;
        private readonly DummySerializable _dummy;

        public RepositoryTests()
        {
            _backupSettings = new BackupSettings(true, Directory, ".bak", null, false, 1, Int32.MaxValue);
            _setting = new RepositorySetting(true, Directory, _backupSettings, ".cfg", ".tmp");

            var name = GetType().Name;

            var fileName = string.Concat(name, _setting.Extension);
            _file = new FileInfo(Path.Combine(_setting.Directory.FullName, fileName));

            var tempfileName = string.Concat(name, _setting.TempExtension);
            _tempFile = new FileInfo(Path.Combine(_setting.Directory.FullName, tempfileName));

            var backupFileName = string.Concat(name, _backupSettings.Extension);
            _backup = new FileInfo(Path.Combine(_backupSettings.Directory.FullName, backupFileName));

            var repoSettingFileName = string.Concat(typeof(RepositorySetting).Name, _setting.Extension);
            RepoSettingFile = new FileInfo(Path.Combine(_setting.Directory.FullName, repoSettingFileName));

            var dummyFileName = string.Concat(typeof(DummySerializable).Name, _setting.Extension);
            _dummyFile = new FileInfo(Path.Combine(_setting.Directory.FullName, dummyFileName));

            _dummyBackup = _dummyFile.ChangeExtension(_backupSettings.Extension);
            _dummy = new DummySerializable(1);
        }

        [SetUp]
        public void SetUp()
        {
            _repository = Create(_setting);
            _file.Delete();
            _tempFile.Delete();
            _backup.Delete();

            _dummyFile.Delete();
            _dummyBackup.Delete();
        }

        [TearDown]
        public void TearDown()
        {
            _file.Delete();
            _tempFile.Delete();
            _backup.Delete();
            RepoSettingFile.Delete();

            _dummyFile.Delete();
            _dummyBackup.Delete();
        }

        [Test]
        public void ReadFile()
        {
            Save(_dummy, _file);
            var read = _repository.Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void ReadFileName()
        {
            Save(_dummy, _dummyFile);
            var read = _repository.Read<DummySerializable>(typeof(DummySerializable).Name);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public async Task ReadAsync()
        {
            Save(_dummy, _file);
            var read = await _repository.ReadAsync<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void ReadType()
        {
            Save(_dummy, _dummyFile);
            var read = _repository.Read<DummySerializable>();
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateType(bool exists)
        {
            if (exists)
            {
                Save(_dummy, _dummyFile);
            }
            var read = _repository.ReadOrCreate<DummySerializable>(() => _dummy);
            AssertFile.Exists(true, _dummyFile);
            Assert.AreEqual(_dummy.Value, read.Value);
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
        public async Task ReadAsyncCaches()
        {
            Save(_dummy, _file);
            var read1 = await _repository.ReadAsync<DummySerializable>(_file);
            var read2 = await _repository.ReadAsync<DummySerializable>(_file);
            Assert.AreSame(read1, read2);
        }

        [Test]
        public void SaveFile()
        {
            _repository.Save(_dummy, _file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void SaveFileName()
        {
            var fileName = typeof(DummySerializable).Name;
            _repository.Save(_dummy, fileName);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(false, _dummyBackup);

            _dummy.Value++;
            _repository.Save(_dummy, fileName);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(true, _dummyBackup);
            var read = Read<DummySerializable>(_dummyFile);
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
        public void SaveType()
        {
            _repository.Save(_dummy);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(false, _dummyBackup);
            var read = Read<DummySerializable>(_dummyFile);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);

            _dummy.Value++;
            _repository.Save(_dummy);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(true, _dummyBackup);
        }


        [Test]
        public async Task SaveAsyncType()
        {
            await _repository.SaveAsync(_dummy);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(false, _dummyBackup);
            var read = Read<DummySerializable>(_dummyFile);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);

            _dummy.Value++;
            await _repository.SaveAsync(_dummy);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(true, _dummyBackup);
        }

        [Test]
        public void SaveCreatesBackup()
        {
            _repository.Save(_dummy, _file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);

            _dummy.Value++;
            _repository.Save(_dummy, _file);

            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public async Task SaveAsyncCreatesBackup()
        {
            await _repository.SaveAsync(_dummy, _file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);

            _dummy.Value++;
            await _repository.SaveAsync(_dummy, _file);

            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void SaveCaches()
        {
            _repository.Save(_dummy, _file);
            var read = _repository.Read<DummySerializable>(_file);
            Assert.AreSame(_dummy, read);
        }


        [Test]
        public async Task SaveAsyncCaches()
        {
            await _repository.SaveAsync(_dummy, _file);
            var read = await _repository.ReadAsync<DummySerializable>(_file);
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

        [Test]
        public void Restore()
        {
            _repository.Save(_dummy, _file);
            _dummy.Value++;
            _repository.Save(_dummy, _file); // Save twice so there is a backup
            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            _repository.Backuper.Restore(_file, _backup);

            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value - 1, read.Value);
        }

        protected abstract IRepository Create(RepositorySetting setting);

        protected abstract void Save<T>(T item, FileInfo file);

        protected abstract T Read<T>(FileInfo file);
    }
}