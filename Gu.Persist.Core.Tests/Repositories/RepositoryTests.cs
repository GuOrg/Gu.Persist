// ReSharper disable HeuristicUnreachableCode

namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    public abstract class RepositoryTests
    {
        public IRepository Repository;
        protected readonly DirectoryInfo Directory;
        protected FileInfo RepoSettingFile;

        private const string NewName = "New";
        private readonly DummySerializable dummy;

        private FileInfo file;
        private FileInfo fileTemp;
        private FileInfo fileSoftDelete;
        private FileInfo fileNewName;

        private FileInfo backup;
        private FileInfo backupSoftDelete;
        private FileInfo backupNewName;

        private FileInfo dummyFile;
        private FileInfo dummyNewName;

        private FileInfo dummySoftDelete;
        private FileInfo dummySoftDeleteNewName;

        private FileInfo dummyBackup;
        private FileInfo dummyBackupNewName;
        private LockedFile lockFile;

        protected RepositoryTests()
        {
            this.Directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().FullName);
            this.dummy = new DummySerializable(1);
        }

        protected RepositorySettings Settings => (RepositorySettings)this.Repository?.Settings;

        protected BackupSettings BackupSettings => this.Settings?.BackupSettings;

        protected bool IsBackingUp => this.BackupSettings != null;

        [SetUp]
        public void SetUp()
        {
            this.Repository = this.Create();
            this.Repository.ClearCache();

            var name = this.GetType()
                           .Name;

            var fileName = string.Concat(name, this.Settings.Extension);
            var settingsDirectory = this.Settings.Directory.CreateDirectoryInfo();
            this.file = settingsDirectory.CreateFileInfoInDirectory(fileName);

            var tempfileName = string.Concat(name, this.Settings.TempExtension);
            this.fileTemp = settingsDirectory.CreateFileInfoInDirectory(tempfileName);
            this.fileSoftDelete = this.file.GetSoftDeleteFileFor();
            this.fileNewName = this.file.WithNewName(NewName, this.Settings);
            if (this.IsBackingUp)
            {
                var backupFileName = string.Concat(name, this.BackupSettings.Extension);
                this.backup = this.BackupSettings.Directory.CreateDirectoryInfo()
                                  .CreateFileInfoInDirectory(backupFileName);
                this.backupSoftDelete = this.backup.GetSoftDeleteFileFor();
                this.backupNewName = this.backup.WithNewName(NewName, this.BackupSettings);
            }

            var repoSettingFileName = string.Concat(
                this.Settings.GetType()
                    .Name,
                this.Settings.Extension);
            this.RepoSettingFile = settingsDirectory.CreateFileInfoInDirectory(repoSettingFileName);

            var dummyFileName = string.Concat(typeof(DummySerializable).Name, this.Settings.Extension);
            this.dummyFile = settingsDirectory.CreateFileInfoInDirectory(dummyFileName);
            this.dummySoftDelete = this.dummyFile.GetSoftDeleteFileFor();
            this.dummySoftDeleteNewName = this.dummySoftDelete.WithNewName(NewName, this.Settings);
            this.dummyNewName = this.dummyFile.WithNewName(NewName, this.Settings);
            if (this.IsBackingUp)
            {
                var backupFileName = this.dummyFile.WithNewExtension(this.BackupSettings.Extension)
                                         .Name;
                this.dummyBackup = this.BackupSettings.Directory.CreateDirectoryInfo()
                                       .CreateFileInfoInDirectory(backupFileName);
                this.dummyBackupNewName = this.dummyBackup.WithNewName(NewName, this.BackupSettings);
            }

            this.file.Delete();
            this.fileTemp.Delete();
            this.fileSoftDelete.Delete();
            this.fileNewName.Delete();
            if (this.IsBackingUp)
            {
                this.backup.Delete();
                this.backupSoftDelete.Delete();
                this.backupNewName.Delete();
            }

            this.dummyFile.Delete();
            this.dummyNewName.Delete();

            this.dummySoftDelete.Delete();
            this.dummySoftDeleteNewName.Delete();
            if (this.IsBackingUp)
            {
                this.dummyBackup.Delete();
                this.dummyBackupNewName.Delete();
            }
        }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var lockFileInfo = Directories.TempDirectory.CreateFileInfoInDirectory("test.lock");
            try
            {
                lockFileInfo.Delete();
            }
            catch
            {
                // this could happen if the previous run was stopped in the debugger.
            }

            // using this because AppVeyor uses two workers for running the tests.
            this.lockFile = await LockedFile.CreateAsync(lockFileInfo, TimeSpan.FromSeconds(1))
                                .ConfigureAwait(false);
        }

        [TearDown]
        public void TearDown()
        {
            this.file?.Delete();
            this.fileTemp?.Delete();
            this.RepoSettingFile?.Delete();
            this.dummyFile?.Delete();
            if (this.IsBackingUp)
            {
                this.backup?.Delete();
                this.dummyBackup?.Delete();
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.lockFile?.DisposeAndDeleteFile();
            if (this.Directory.Exists)
            {
                this.Directory.Delete(true);
            }

            if (this.file?.Directory?.Exists == true)
            {
                this.file.Directory.Delete(true);
            }
        }

        [Test]
        public void ReadFile()
        {
            this.Save(this.file, this.dummy);
            var read = this.Repository.Read<DummySerializable>(this.file);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void ReadFileName()
        {
            this.Save(this.dummyFile, this.dummy);
            var read = this.Repository.Read<DummySerializable>(typeof(DummySerializable).Name);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public async Task ReadAsync()
        {
            this.Save(this.file, this.dummy);
            var read = await this.Repository.ReadAsync<DummySerializable>(this.file).ConfigureAwait(false);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void ReadType()
        {
            this.Save(this.dummyFile, this.dummy);
            var read = this.Repository.Read<DummySerializable>();
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateType(bool exists)
        {
            if (exists)
            {
                this.Save(this.dummyFile, this.dummy);
            }

            var read = this.Repository.ReadOrCreate(() => this.dummy);
            AssertFile.Exists(true, this.dummyFile);
            Assert.AreEqual(this.dummy.Value, read.Value);
        }

        [Test]
        public void ReadCaches()
        {
            this.Save(this.file, this.dummy);
            var read1 = this.Repository.Read<DummySerializable>(this.file);
            var read2 = this.Repository.Read<DummySerializable>(this.file);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public async Task ReadAsyncCaches()
        {
            this.Save(this.file, this.dummy);
            var read1 = await this.Repository.ReadAsync<DummySerializable>(this.file).ConfigureAwait(false);
            var read2 = await this.Repository.ReadAsync<DummySerializable>(this.file).ConfigureAwait(false);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public void SaveThenRead()
        {
            this.Repository.Save(this.file, this.dummy);
            AssertFile.Exists(true, this.file);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.backup);
            }

            var read = this.Read<DummySerializable>(this.file);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void SaveNull()
        {
            this.Repository.Save(this.file, this.dummy);
            AssertFile.Exists(true, this.file);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.backup);
            }

            if (this.Settings.SaveNullDeletesFile)
            {
                this.Repository.Save<DummySerializable>(this.file, null);
                AssertFile.Exists(false, this.file);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.backup);
                }
            }
            else
            {
                Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(this.file, null));
            }
        }

        [Test]
        public void SaveLongListThenShortListFile()
        {
            var list = new List<DummySerializable>
            {
                this.dummy,
                new DummySerializable(2)
            };
            this.Repository.Save(this.file, list);
            AssertFile.Exists(true, this.file);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.backup);
            }

            var read = this.Read<List<DummySerializable>>(this.file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(this.dummy, read);

            list.RemoveAt(1);
            this.Repository.Save(this.file, list);
            AssertFile.Exists(true, this.file);
            read = this.Read<List<DummySerializable>>(this.file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(this.dummy, read);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Delete(bool deleteBakups)
        {
            this.dummyFile.VoidCreate();
            this.dummySoftDelete.VoidCreate();
            if (this.IsBackingUp)
            {
                this.dummyBackup.VoidCreate();
                AssertFile.Exists(true, this.dummyBackup);
            }

            AssertFile.Exists(true, this.dummyFile);
            AssertFile.Exists(true, this.dummySoftDelete);

            this.Repository.Delete<DummySerializable>(deleteBakups);
            AssertFile.Exists(false, this.dummyFile);
            AssertFile.Exists(false, this.dummySoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(!deleteBakups, this.dummyBackup);
            }
        }

        [Test]
        public void DeleteBackups()
        {
            if (this.Repository.Backuper is NullBackuper)
            {
                return;
            }

            this.dummySoftDelete.VoidCreate();
            if (this.IsBackingUp)
            {
                this.dummyBackup.VoidCreate();
            }

            AssertFile.Exists(true, this.dummySoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.dummyBackup);
            }

            this.Repository.DeleteBackups<DummySerializable>();
            AssertFile.Exists(false, this.dummySoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.dummyBackup);
            }
        }

        [Test]
        public void SaveFileName()
        {
            var fileName = typeof(DummySerializable).Name;
            this.Repository.Save(fileName, this.dummy);
            AssertFile.Exists(true, this.dummyFile);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.dummyBackup);
            }

            this.dummy.Value++;
            this.Repository.Save(fileName, this.dummy);
            AssertFile.Exists(true, this.dummyFile);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.dummyBackup);
            }

            var read = this.Read<DummySerializable>(this.dummyFile);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void SaveFileNameNull()
        {
            var fileName = typeof(DummySerializable).Name;
            this.Repository.Save(fileName, this.dummy);
            var fileInfo = this.Repository.GetFileInfo(fileName);
            AssertFile.Exists(true, fileInfo);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.dummyBackup);
            }

            if (this.Settings.SaveNullDeletesFile)
            {
                this.Repository.Save<DummySerializable>(fileName, null);
                AssertFile.Exists(false, fileInfo);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.dummyBackup);
                }
            }
            else
            {
                Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(fileName, null));
            }
        }

        [Test]
        public async Task SaveAsync()
        {
            await this.Repository.SaveAsync(this.file, this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.file);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.backup);
            }

            var read = this.Read<DummySerializable>(this.file);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public async Task SaveAsyncNull()
        {
            await this.Repository.SaveAsync(this.file, this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.file);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.backup);
            }

            if (this.Settings.SaveNullDeletesFile)
            {
                await this.Repository.SaveAsync<DummySerializable>(this.file, null).ConfigureAwait(false);
                AssertFile.Exists(false, this.file);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.backup);
                }
            }
            else
            {
                Assert.ThrowsAsync<ArgumentNullException>(() => this.Repository.SaveAsync<DummySerializable>(this.file, null));
            }
        }

        [Test]
        public void SaveType()
        {
            this.Repository.Save(this.dummy);
            AssertFile.Exists(true, this.dummyFile);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.dummyBackup);
            }

            var read = this.Read<DummySerializable>(this.dummyFile);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);

            this.dummy.Value++;
            this.Repository.Save(this.dummy);
            AssertFile.Exists(true, this.dummyFile);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.dummyBackup);
            }
        }

        [Test]
        public void SaveTypeNull()
        {
            this.Repository.Save(this.dummy);
            AssertFile.Exists(true, this.dummyFile);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.dummyBackup);
            }

            var read = this.Read<DummySerializable>(this.dummyFile);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);

            if (this.Settings.SaveNullDeletesFile)
            {
                this.Repository.Save<DummySerializable>(null);
                AssertFile.Exists(false, this.file);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.dummyBackup);
                }
            }
            else
            {
                Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(null));
            }
        }

        [Test]
        public async Task SaveAsyncType()
        {
            await this.Repository.SaveAsync(this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.dummyFile);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.dummyBackup);
            }

            var read = this.Read<DummySerializable>(this.dummyFile);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);

            this.dummy.Value++;
            await this.Repository.SaveAsync(this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.dummyFile);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.dummyBackup);
            }
        }

        [Test]
        public async Task SaveAsyncTypeNull()
        {
            await this.Repository.SaveAsync(this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.dummyFile);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.dummyBackup);
            }

            var read = this.Read<DummySerializable>(this.dummyFile);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);

            if (this.Settings.SaveNullDeletesFile)
            {
                await this.Repository.SaveAsync<DummySerializable>(null).ConfigureAwait(false);
                AssertFile.Exists(false, this.file);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.dummyBackup);
                }
            }
            else
            {
                Assert.ThrowsAsync<ArgumentNullException>(() => this.Repository.SaveAsync<DummySerializable>(null));
            }
        }

        [Test]
        public void SaveCreatesBackup()
        {
            this.Repository.Save(this.file, this.dummy);
            AssertFile.Exists(true, this.file);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.backup);
            }

            this.dummy.Value++;
            this.Repository.Save(this.file, this.dummy);

            AssertFile.Exists(true, this.file);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.backup);
            }

            var read = this.Read<DummySerializable>(this.file);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public async Task SaveAsyncCreatesBackup()
        {
            await this.Repository.SaveAsync(this.file, this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.file);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.backup);
            }

            this.dummy.Value++;
            await this.Repository.SaveAsync(this.file, this.dummy).ConfigureAwait(false);

            AssertFile.Exists(true, this.file);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.backup);
            }

            var read = this.Read<DummySerializable>(this.file);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void SaveCaches()
        {
            this.Repository.Save(this.file, this.dummy);
            var read = this.Repository.Read<DummySerializable>(this.file);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy, read);
                Assert.AreNotSame(this.dummy, read);
            }
        }

        [Test]
        public async Task SaveAsyncCaches()
        {
            await this.Repository.SaveAsync(this.file, this.dummy).ConfigureAwait(false);
            var read = await this.Repository.ReadAsync<DummySerializable>(this.file).ConfigureAwait(false);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy, read);
                Assert.AreNotSame(this.dummy, read);
            }
        }

        [Test]
        public void SaveThreeTimes()
        {
            this.Repository.Save(this.file, this.dummy);
            var read = this.Repository.Read<DummySerializable>(this.file);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy, read);
                Assert.AreNotSame(this.dummy, read);
            }

            read = this.Read<DummySerializable>(this.file);
            Assert.AreEqual(this.dummy, read);
            for (var i = 2; i < 3; i++)
            {
                this.dummy.Value++;
                this.Repository.Save(this.file, this.dummy);
                read = this.Repository.Read<DummySerializable>(this.file);
                if (this.Repository is ISingletonRepository)
                {
                    Assert.AreSame(this.dummy, read);
                }
                else
                {
                    Assert.AreEqual(this.dummy, read);
                    Assert.AreNotSame(this.dummy, read);
                }

                read = this.Read<DummySerializable>(this.file);
                Assert.AreEqual(this.dummy, read);
                if (this.IsBackingUp)
                {
                    read = this.Read<DummySerializable>(this.backup);
                    Assert.AreEqual(this.dummy.Value - 1, read.Value);
                }
            }
        }

        [Test]
        public void IsDirtyType()
        {
            if (this.Settings.IsTrackingDirty)
            {
                Assert.IsTrue(this.Repository.IsDirty(this.dummy));

                this.Repository.Save(this.dummy);
                Assert.IsFalse(this.Repository.IsDirty(this.dummy));

                this.dummy.Value++;
                Assert.IsTrue(this.Repository.IsDirty(this.dummy));
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => this.Repository.IsDirty(this.dummy));
            }
        }

        [Test]
        public void IsDirtyFileName()
        {
            var fileName = this.dummy.GetType().Name;
            if (this.Settings.IsTrackingDirty)
            {
                Assert.IsTrue(this.Repository.IsDirty(fileName, this.dummy));

                this.Repository.Save(fileName, this.dummy);
                Assert.IsFalse(this.Repository.IsDirty(fileName, this.dummy));

                this.dummy.Value++;
                Assert.IsTrue(this.Repository.IsDirty(fileName, this.dummy));
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => this.Repository.IsDirty(fileName, this.dummy));
            }
        }

        [Test]
        public void IsDirtyFile()
        {
            if (this.Settings.IsTrackingDirty)
            {
                Assert.IsTrue(this.Repository.IsDirty(this.dummyFile, this.dummy));

                this.Repository.Save(this.dummyFile, this.dummy);
                Assert.IsFalse(this.Repository.IsDirty(this.dummyFile, this.dummy));

                this.dummy.Value++;
                Assert.IsTrue(this.Repository.IsDirty(this.dummyFile, this.dummy));
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => this.Repository.IsDirty(this.dummyFile, this.dummy));
            }
        }

        [Test]
        public void CanRenameTypeHappyPath()
        {
            this.dummyFile.VoidCreate();
            if (this.IsBackingUp)
            {
                this.dummyBackup.VoidCreate();
            }

            Assert.IsTrue(this.Repository.CanRename<DummySerializable>(NewName));
        }

        [Test]
        public void CanRenameFileNameHappyPath()
        {
            this.dummyFile.VoidCreate();
            if (this.IsBackingUp)
            {
                this.dummyBackup.VoidCreate();
            }

            var oldName = nameof(DummySerializable);
            Assert.IsTrue(this.Repository.CanRename(oldName, NewName));
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanRenameTypeWouldOverwrite(bool fileNewNameExists, bool backupNewNameExists)
        {
            this.dummyFile.VoidCreate();
            if (fileNewNameExists)
            {
                this.dummyNewName.VoidCreate();
            }

            if (this.IsBackingUp)
            {
                this.dummyBackup.VoidCreate();
            }

            if (backupNewNameExists)
            {
                if (this.IsBackingUp)
                {
                    this.dummyBackupNewName.VoidCreate();
                }

                this.dummySoftDelete.VoidCreate();
                this.dummySoftDeleteNewName.VoidCreate();
            }

            Assert.IsFalse(this.Repository.CanRename<DummySerializable>(NewName));
        }

        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void RenameType(bool hasBackup, bool hasSoft)
        {
            if (!this.IsBackingUp && hasBackup)
            {
                Assert.Inconclusive("Not a relevant test for this config");
                return; // due to inheritance
            }

            this.dummyFile.VoidCreate();
            if (hasBackup)
            {
                this.dummyBackup.VoidCreate();
            }

            if (hasSoft)
            {
                this.dummySoftDelete.VoidCreate();
            }

            this.Repository.Rename<DummySerializable>(NewName, false);
            AssertFile.Exists(true, this.dummyNewName);
            if (hasBackup)
            {
                AssertFile.Exists(true, this.dummyBackupNewName);
            }

            if (hasSoft)
            {
                AssertFile.Exists(true, this.dummySoftDeleteNewName);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RenameTypeWouldOverwrite(bool owerWrite)
        {
            this.dummyFile.WriteAllText("old");
            this.dummyNewName.WriteAllText("new");
            if (owerWrite)
            {
                this.Repository.Rename<DummySerializable>(NewName, true);
                AssertFile.Exists(false, this.dummyFile);
                AssertFile.Exists(true, this.dummyNewName);
                Assert.AreEqual("old", this.dummyNewName.ReadAllText());
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => this.Repository.Rename<DummySerializable>(NewName, false));
            }
        }

        ////[Test]
        ////public void Restore()
        ////{
        ////    Assert.Inconclusive("Not sure how to solve this and caching. Don't want to do reflection and copy properties I think");
        ////    Repository.Save(_dummy, _file);
        ////    _dummy.Value++;
        ////    Repository.Save(_dummy, _file); // Save twice so there is a backup
        ////    AssertFile.Exists(true, _file);
        ////    AssertFile.Exists(true, _backup);
        ////    Repository.Backuper.Restore(_file, _backup);

        ////    AssertFile.Exists(true, _file);
        ////    AssertFile.Exists(false, _backup);
        ////    var read = Read<DummySerializable>(_file);
        ////    Assert.AreEqual(_dummy.Value - 1, read.Value);
        ////}

        protected abstract IRepository Create();

        protected abstract void Save<T>(FileInfo file, T item);

        protected abstract T Read<T>(FileInfo file);
    }
}