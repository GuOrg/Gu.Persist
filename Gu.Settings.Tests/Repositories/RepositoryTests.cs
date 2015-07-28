namespace Gu.Settings.Tests.Repositories
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Settings.Backup;

    using NUnit.Framework;
    using Settings.IO;

    [RequiresSTA]
    public abstract class RepositoryTests
    {
        private const string NewName = "New";
        protected readonly DirectoryInfo Directory;
        private FileInfo _file;
        private FileInfo _fileTemp;
        private FileInfo _fileSoftDelete;
        private FileInfo _fileNewName;

        private FileInfo _backup;
        private FileInfo _backupSoftDelete;
        private FileInfo _backupNewName;

        protected FileInfo RepoSettingFile;

        private FileInfo _dummyFile;
        private FileInfo _dummyNewName;

        private FileInfo _dummySoftDelete;
        private FileInfo _dummySoftDeleteNewName;

        private FileInfo _dummyBackup;
        private FileInfo _dummyBackupNewName;

        public IRepository Repository;

        protected RepositorySettings Settings
        {
            get { return (RepositorySettings)Repository.Settings; }
        }

        protected BackupSettings BackupSettings
        {
            get { return Repository.Settings.BackupSettings; }
        }

        protected bool IsBackingUp
        {
            get { return BackupSettings != null && BackupSettings.IsCreatingBackups; }
        }

        private readonly DummySerializable _dummy;


        protected RepositoryTests()
        {
            Directory = new DirectoryInfo(@"C:\Temp\Gu.Settings\" + GetType().Name);
            Directories.Default = Directory;
            _dummy = new DummySerializable(1);
        }

        [SetUp]
        public void SetUp()
        {
            var backupSettings = new BackupSettings(Directory, true, BackupSettings.DefaultExtension, null, false, 1, Int32.MaxValue);
            var settings = new RepositorySettings(Directory, true, true, backupSettings, ".cfg", ".tmp");
            Repository = Create(settings);
            Repository.ClearCache();

            var name = GetType().Name;

            var fileName = string.Concat(name, Settings.Extension);
            _file = Settings.Directory.CreateFileInfoInDirectory(fileName);

            var tempfileName = string.Concat(name, Settings.TempExtension);
            _fileTemp = Settings.Directory.CreateFileInfoInDirectory(tempfileName);
            _fileSoftDelete = _file.GetSoftDeleteFileFor();
            _fileNewName = _file.WithNewName(NewName, Settings);

            var backupFileName = string.Concat(name, BackupSettings.DefaultExtension);
            _backup = Directory.CreateFileInfoInDirectory(backupFileName);
            _backupSoftDelete = _backup.GetSoftDeleteFileFor();
            _backupNewName = _backup.WithNewName(NewName, Settings);

            var repoSettingFileName = string.Concat(typeof(RepositorySettings).Name, Settings.Extension);
            RepoSettingFile = Settings.Directory.CreateFileInfoInDirectory(repoSettingFileName);

            var dummyFileName = string.Concat(typeof(DummySerializable).Name, Settings.Extension);
            _dummyFile = Settings.Directory.CreateFileInfoInDirectory(dummyFileName);
            _dummySoftDelete = _dummyFile.GetSoftDeleteFileFor();
            _dummySoftDeleteNewName = _dummySoftDelete.WithNewName(NewName, Settings);
            _dummyNewName = _dummyFile.WithNewName(NewName, Settings);
            _dummyBackup = _dummyFile.WithNewExtension(BackupSettings.DefaultExtension);
            _dummyBackupNewName = _dummyBackup.WithNewName(NewName, Settings);

            _file.Delete();
            _fileTemp.Delete();
            _fileSoftDelete.Delete();
            _fileNewName.Delete();

            _backup.Delete();
            _backupSoftDelete.Delete();
            _backupNewName.Delete();

            _dummyFile.Delete();
            _dummyNewName.Delete();

            _dummySoftDelete.Delete();
            _dummySoftDeleteNewName.Delete();

            _dummyBackup.Delete();
            _dummyBackupNewName.Delete();
        }

        [TearDown]
        public void TearDown()
        {
            _file.Delete();
            _fileTemp.Delete();
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
            var read1 = await Repository.ReadAsync<DummySerializable>(_file).ConfigureAwait(false);
            var read2 = await Repository.ReadAsync<DummySerializable>(_file).ConfigureAwait(false);
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

        [TestCase(true)]
        [TestCase(false)]
        public void Delete(bool deleteBakups)
        {
            _dummyFile.VoidCreate();
            _dummySoftDelete.VoidCreate();
            if (!(Repository.Backuper is NullBackuper))
            {
                _dummyBackup.VoidCreate();
                AssertFile.Exists(true, _dummyBackup);
            } 
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(true, _dummySoftDelete);

            Repository.Delete<DummySerializable>(deleteBakups);
            AssertFile.Exists(false, _dummyFile);
            AssertFile.Exists(false, _dummySoftDelete);
            if (!(Repository.Backuper is NullBackuper))
            {
                AssertFile.Exists(!deleteBakups, _dummyBackup);
            }
        }

        [Test]
        public void DeleteBackups()
        {
            if (Repository.Backuper is NullBackuper)
            {
                return;
            }
            _dummySoftDelete.VoidCreate();
            _dummyBackup.VoidCreate();
            AssertFile.Exists(true, _dummySoftDelete);
            AssertFile.Exists(true, _dummyBackup);
            Repository.DeleteBackups<DummySerializable>();
            AssertFile.Exists(false, _dummySoftDelete);
            AssertFile.Exists(false, _dummyBackup);
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
            var createsBackups = Settings.BackupSettings != null && Settings.BackupSettings.IsCreatingBackups;
            AssertFile.Exists(createsBackups, _dummyBackup);
            var read = Read<DummySerializable>(_dummyFile);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public async Task SaveAsync()
        {
            await Repository.SaveAsync(_dummy, _file).ConfigureAwait(false);
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
            var createsBackups = Settings.BackupSettings != null && Settings.BackupSettings.IsCreatingBackups;
            AssertFile.Exists(createsBackups, _dummyBackup);
        }

        [Test]
        public async Task SaveAsyncType()
        {
            await Repository.SaveAsync(_dummy).ConfigureAwait(false);
            AssertFile.Exists(true, _dummyFile);
            AssertFile.Exists(false, _dummyBackup);
            var read = Read<DummySerializable>(_dummyFile);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);

            _dummy.Value++;
            await Repository.SaveAsync(_dummy).ConfigureAwait(false);
            AssertFile.Exists(true, _dummyFile);
            var createsBackups = Settings.BackupSettings != null && Settings.BackupSettings.IsCreatingBackups;
            AssertFile.Exists(createsBackups, _dummyBackup);
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
            var createsBackups = Settings.BackupSettings != null && Settings.BackupSettings.IsCreatingBackups;
            AssertFile.Exists(createsBackups, _backup);

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
            var createsBackups = Settings.BackupSettings != null && Settings.BackupSettings.IsCreatingBackups;
            AssertFile.Exists(createsBackups, _backup);
            var read = Read<DummySerializable>(_file);
            Assert.AreEqual(_dummy.Value, read.Value);
            Assert.AreNotSame(_dummy, read);
        }

        [Test]
        public void SaveCaches()
        {
            Repository.Save(_dummy, _file);
            var read = Repository.Read<DummySerializable>(_file);
            if (Settings.IsCaching)
            {
                Assert.AreSame(_dummy, read);
            }
            else
            {
                Assert.AreEqual(_dummy, read);
                Assert.AreNotSame(_dummy, read);
            }
        }

        [Test]
        public async Task SaveAsyncCaches()
        {
            await Repository.SaveAsync(_dummy, _file).ConfigureAwait(false);
            var read = await Repository.ReadAsync<DummySerializable>(_file).ConfigureAwait(false);
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
            var createsBackups = Settings.BackupSettings != null && Settings.BackupSettings.IsCreatingBackups;

            for (int i = 2; i < 3; i++)
            {
                _dummy.Value++;
                Repository.Save(_dummy, _file);
                read = Repository.Read<DummySerializable>(_file);
                Assert.AreSame(_dummy, read);
                read = Read<DummySerializable>(_file);
                Assert.AreEqual(_dummy, read);
                if (createsBackups)
                {
                    read = Read<DummySerializable>(_backup);
                    Assert.AreEqual(_dummy.Value - 1, read.Value);
                }
                else
                {
                    AssertFile.Exists(false, _backup);
                }
            }
        }

        [Test]
        public void IsDirtyType()
        {
            Assert.IsTrue(Repository.IsDirty(_dummy));

            Repository.Save(_dummy);
            Assert.IsFalse(Repository.IsDirty(_dummy));

            _dummy.Value++;
            Assert.IsTrue(Repository.IsDirty(_dummy));
        }

        [Test]
        public void IsDirtyFileName()
        {
            var fileName = _dummy.GetType().Name;
            Assert.IsTrue(Repository.IsDirty(_dummy, fileName));

            Repository.Save(_dummy, fileName);
            Assert.IsFalse(Repository.IsDirty(_dummy, fileName));

            _dummy.Value++;
            Assert.IsTrue(Repository.IsDirty(_dummy, fileName));
        }

        [Test]
        public void IsDirtyFile()
        {
            Assert.IsTrue(Repository.IsDirty(_dummy, _dummyFile));

            Repository.Save(_dummy, _dummyFile);
            Assert.IsFalse(Repository.IsDirty(_dummy, _dummyFile));

            _dummy.Value++;
            Assert.IsTrue(Repository.IsDirty(_dummy, _dummyFile));
        }

        [Test]
        public void CanRenameTypeHappyPath()
        {
            _dummyFile.VoidCreate();
            _dummyBackup.VoidCreate();
            Assert.IsTrue(Repository.CanRename<DummySerializable>(NewName));
        }

        [Test]
        public void CanRenameFileNamePath()
        {
            _dummyFile.VoidCreate();
            _dummyBackup.VoidCreate();
            Assert.IsTrue(Repository.CanRename(typeof(DummySerializable).Name, NewName));
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanRenameTypeWouldOverwrite(bool fileNewNameExists, bool backupNewNameExists)
        {
            _dummyFile.VoidCreate();
            if (fileNewNameExists)
            {
                _dummyNewName.VoidCreate();
            }

            _dummyBackup.VoidCreate();
            if (backupNewNameExists)
            {
                _dummyBackupNewName.VoidCreate();
                _dummySoftDelete.VoidCreate();
                _dummySoftDeleteNewName.VoidCreate();
            }
            Assert.IsFalse(Repository.CanRename<DummySerializable>(NewName));
        }

        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void RenameType(bool hasBackup, bool hasSoft)
        {
            if (!IsBackingUp && hasBackup)
            {
                Assert.Inconclusive("Not a relevant test for this config");
                return; // due to inheritance
            }
            _dummyFile.VoidCreate();
            if (hasBackup)
            {
                _dummyBackup.VoidCreate();
            }
            if (hasSoft)
            {
                _dummySoftDelete.VoidCreate();
            }

            Repository.Rename<DummySerializable>(NewName, false);
            AssertFile.Exists(true, _dummyNewName);
            if (hasBackup)
            {
                AssertFile.Exists(true, _dummyBackupNewName);
            }

            if (hasSoft)
            {
                AssertFile.Exists(true, _dummySoftDeleteNewName);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RenameTypeWouldOverwrite(bool owerWrite)
        {
            _dummyFile.WriteAllText("old");
            _dummyNewName.WriteAllText("new");
            if (owerWrite)
            {
                Repository.Rename<DummySerializable>(NewName, true);
                AssertFile.Exists(false, _dummyFile);
                AssertFile.Exists(true, _dummyNewName);
                Assert.AreEqual("old", _dummyNewName.ReadAllText());
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => Repository.Rename<DummySerializable>(NewName, false));
            }
        }

        [Test]
        public void Restore()
        {
            Assert.Inconclusive("Not sure how to solve this and caching. Don't want to do reflection and copy properties I think");
            //Repository.Save(_dummy, _file);
            //_dummy.Value++;
            //Repository.Save(_dummy, _file); // Save twice so there is a backup
            //AssertFile.Exists(true, _file);
            //AssertFile.Exists(true, _backup);
            //Repository.Backuper.Restore(_file, _backup);

            //AssertFile.Exists(true, _file);
            //AssertFile.Exists(false, _backup);
            //var read = Read<DummySerializable>(_file);
            //Assert.AreEqual(_dummy.Value - 1, read.Value);
        }

        protected abstract IRepository Create(RepositorySettings settings);

        protected abstract void Save<T>(T item, FileInfo file);

        protected abstract T Read<T>(FileInfo file);
    }
}