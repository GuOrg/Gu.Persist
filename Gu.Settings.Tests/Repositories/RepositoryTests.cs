namespace Gu.Settings.Tests.Repositories
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using NUnit.Framework;
    using Settings.IO;

    [RequiresSTA]
    public abstract class RepositoryTests
    {
        protected readonly DirectoryInfo Directory;
        private readonly FileInfo _file;
        private readonly FileInfo _tempFile;
        private readonly FileInfo _backup;
        protected readonly FileInfo RepoSettingFile;

        private readonly FileInfo _dummyFile;
        private readonly FileInfo _dummyBackup;

        protected readonly RepositorySettings Settings;
        public IRepository Repository;
        private readonly DummySerializable _dummy;

        protected RepositoryTests()
        {
            Directory = new DirectoryInfo(@"C:\Temp\Gu.Settings\" + GetType().Name);
            Directories.Default = Directory;
            var backupSettings = new BackupSettings(Directory, true, BackupSettings.DefaultExtension, null, false, 1, Int32.MaxValue);
            Settings = new RepositorySettings(Directory, true, true, backupSettings, ".cfg", ".tmp");

            var name = GetType().Name;

            var fileName = string.Concat(name, Settings.Extension);
            _file = Settings.Directory.CreateFileInfoInDirectory(fileName);

            var tempfileName = string.Concat(name, Settings.TempExtension);
            _tempFile = Settings.Directory.CreateFileInfoInDirectory(tempfileName);

            var backupFileName = string.Concat(name, backupSettings.Extension);
            _backup = backupSettings.Directory.CreateFileInfoInDirectory(backupFileName);

            var repoSettingFileName = string.Concat(typeof(RepositorySettings).Name, Settings.Extension);
            RepoSettingFile = Settings.Directory.CreateFileInfoInDirectory(repoSettingFileName);

            var dummyFileName = string.Concat(typeof(DummySerializable).Name, Settings.Extension);
            _dummyFile = Settings.Directory.CreateFileInfoInDirectory(dummyFileName);

            _dummyBackup = _dummyFile.ChangeExtension(backupSettings.Extension);
            _dummy = new DummySerializable(1);
        }

        [SetUp]
        public void SetUp()
        {
            Repository = Create(Settings);
            Repository.ClearCache();
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
            var read = Repository.Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void ReadFileName()
        {
            Save(_dummy, _dummyFile);
            var read = Repository.Read<DummySerializable>(typeof(DummySerializable).Name);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public async Task ReadAsync()
        {
            Save(_dummy, _file);
            var read = await Repository.ReadAsync<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void ReadType()
        {
            Save(_dummy, _dummyFile);
            var read = Repository.Read<DummySerializable>();
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
            var read = Repository.ReadOrCreate<DummySerializable>(() => _dummy);
            AssertFile.Exists(true, _dummyFile);
            Assert.AreEqual(_dummy.Value, read.Value);
        }

        [Test]
        public void ReadCaches()
        {
            Save(_dummy, _file);
            var read1 = Repository.Read<DummySerializable>(_file);
            var read2 = Repository.Read<DummySerializable>(_file);
            Assert.AreSame(read1, read2);
        }


        [Test]
        public async Task ReadAsyncCaches()
        {
            Save(_dummy, _file);
            var read1 = await Repository.ReadAsync<DummySerializable>(_file);
            var read2 = await Repository.ReadAsync<DummySerializable>(_file);
            Assert.AreSame(read1, read2);
        }

        [Test]
        public void SaveFile()
        {
            Repository.Save(_dummy, _file);
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
            Repository.Save(_dummy, fileName);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(false, _dummyBackup);

            _dummy.Value++;
            Repository.Save(_dummy, fileName);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(true, _dummyBackup);
            var read = Read<DummySerializable>(_dummyFile);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public async Task SaveAsync()
        {
            await Repository.SaveAsync(_dummy, _file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void SaveType()
        {
            Repository.Save(_dummy);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(false, _dummyBackup);
            var read = Read<DummySerializable>(_dummyFile);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);

            _dummy.Value++;
            Repository.Save(_dummy);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(true, _dummyBackup);
        }

        [Test]
        public async Task SaveAsyncType()
        {
            await Repository.SaveAsync(_dummy);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(false, _dummyBackup);
            var read = Read<DummySerializable>(_dummyFile);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);

            _dummy.Value++;
            await Repository.SaveAsync(_dummy);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(true, _dummyBackup);
        }

        [Test]
        public void SaveCreatesBackup()
        {
            Repository.Save(_dummy, _file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);

            _dummy.Value++;
            Repository.Save(_dummy, _file);

            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public async Task SaveAsyncCreatesBackup()
        {
            await Repository.SaveAsync(_dummy, _file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);

            _dummy.Value++;
            await Repository.SaveAsync(_dummy, _file);

            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void SaveCaches()
        {
            Repository.Save(_dummy, _file);
            var read = Repository.Read<DummySerializable>(_file);
            Assert.AreSame(_dummy, read);
        }

        [Test]
        public async Task SaveAsyncCaches()
        {
            await Repository.SaveAsync(_dummy, _file);
            var read = await Repository.ReadAsync<DummySerializable>(_file);
            Assert.AreSame(_dummy, read);
        }

        [Test]
        public void SaveThreeTimes()
        {
            Repository.Save(_dummy, _file);
            var read = Repository.Read<DummySerializable>(_file);
            Assert.AreSame(_dummy, read);
            read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy, read);

            for (int i = 2; i < 3; i++)
            {
                _dummy.Value++;
                Repository.Save(_dummy, _file);
                read = Repository.Read<DummySerializable>(_file);
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
            Assert.IsTrue(Repository.IsDirty(_dummy));

            Repository.Save(_dummy);
            Assert.IsFalse(Repository.IsDirty(_dummy));

            _dummy.Value++;
            Assert.IsTrue(Repository.IsDirty(_dummy));
        }

        [Test]
        public void Restore()
        {
            Repository.Save(_dummy, _file);
            _dummy.Value++;
            Repository.Save(_dummy, _file); // Save twice so there is a backup
            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            Repository.Backuper.Restore(_file, _backup);

            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value - 1, read.Value);
        }

        protected abstract IRepository Create(RepositorySettings settings);

        protected abstract void Save<T>(T item, FileInfo file);

        protected abstract T Read<T>(FileInfo file);
    }
}