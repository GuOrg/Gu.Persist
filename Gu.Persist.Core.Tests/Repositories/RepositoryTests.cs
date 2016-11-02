namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Persist.Core;

    using NUnit.Framework;

    public abstract class RepositoryTests
    {
        private readonly DummySerializable dummy;
        private LockedFile lockFile;

        protected RepositoryTests()
        {
            this.dummy = new DummySerializable(1);
        }

        public RepositorySettings Settings => (RepositorySettings)this.Repository?.Settings;

        public BackupSettings BackupSettings => this.Settings?.BackupSettings;

        public bool IsBackingUp => this.BackupSettings != null;

        public IRepository Repository { get; private set; }

        public DirectoryInfo Directory => this.Repository?.Settings.Directory != null
                                              ? new DirectoryInfo(this.Repository.Settings.Directory)
                                              : null;

        public DirectoryInfo TargetDirectory => new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().FullName);

        public Files NamedFiles { get; private set; }

        public Files TypeFiles { get; private set; }

        public FileInfo RepoSettingFile { get; private set; }

        [SetUp]
        public void SetUp()
        {
            this.Directory?.DeleteIfExists(true);
            this.TargetDirectory?.DeleteIfExists(true);
            this.Repository = this.Create();
            this.Repository.ClearCache();
            this.NamedFiles = new Files(this.GetType().Name, this.Settings);
            this.TypeFiles = new Files(this.dummy.GetType().Name, this.Settings);
            this.RepoSettingFile = this.Directory.CreateFileInfoInDirectory(string.Concat(this.Settings.GetType().Name, this.Settings.Extension));
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
            this.Directory.DeleteIfExists(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.lockFile?.DisposeAndDeleteFile();
        }

        [Test]
        public void ReadFileType()
        {
            this.Save(this.TypeFiles.File, this.dummy);
            var read = this.Repository.Read<DummySerializable>();
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void ReadFileName()
        {
            this.Save(this.NamedFiles.File, this.dummy);
            var read = this.Repository.Read<DummySerializable>(this.NamedFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public async Task ReadAsync()
        {
            this.Save(this.NamedFiles.File, this.dummy);
            var read = await this.Repository.ReadAsync<DummySerializable>(this.NamedFiles.File).ConfigureAwait(false);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void ReadType()
        {
            this.Save(this.TypeFiles.File, this.dummy);
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
                this.Save(this.TypeFiles.File, this.dummy);
            }

            var read = this.Repository.ReadOrCreate(() => this.dummy);
            AssertFile.Exists(true, this.TypeFiles.File);
            if (exists)
            {
                Assert.AreNotSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy.Value, read.Value);
                Assert.AreSame(this.dummy, read);
            }
        }

        [Test]
        public void ReadTypeCaches()
        {
            this.Save(this.TypeFiles.File, this.dummy);
            var read1 = this.Repository.Read<DummySerializable>();
            var read2 = this.Repository.Read<DummySerializable>();
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
        public void ReadNamedCaches()
        {
            this.Save(this.NamedFiles.File, this.dummy);
            var read1 = this.Repository.Read<DummySerializable>(this.NamedFiles.File);
            var read2 = this.Repository.Read<DummySerializable>(this.NamedFiles.File);
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
        public async Task ReadTypeAsyncCaches()
        {
            this.Save(this.TypeFiles.File, this.dummy);
            var read1 = await this.Repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
            var read2 = await this.Repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
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
        public async Task ReadNameAsyncCaches()
        {
            this.Save(this.NamedFiles.File, this.dummy);
            var read1 = await this.Repository.ReadAsync<DummySerializable>(this.NamedFiles.File).ConfigureAwait(false);
            var read2 = await this.Repository.ReadAsync<DummySerializable>(this.NamedFiles.File).ConfigureAwait(false);
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
        public void SaveThenReadType()
        {
            AssertFile.Exists(false, this.TypeFiles.File);
            this.Repository.Save(this.dummy);
            AssertFile.Exists(true, this.TypeFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.TypeFiles.Backup);
            }

            var read = this.Repository.Read<DummySerializable>();
            Assert.AreEqual(this.dummy.Value, read.Value);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreNotSame(this.dummy, read);
            }
        }

        [Test]
        public void SaveThenReadName()
        {
            AssertFile.Exists(false, this.NamedFiles.File);
            this.Repository.Save(this.NamedFiles.File, this.dummy);
            AssertFile.Exists(true, this.NamedFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.NamedFiles.Backup);
            }

            var read = this.Repository.Read<DummySerializable>(this.NamedFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreNotSame(this.dummy, read);
            }
        }

        [Test]
        public void SaveNullType()
        {
            this.Repository.Save(this.dummy);
            AssertFile.Exists(true, this.TypeFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.TypeFiles.Backup);
            }

            if ((this.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                this.Repository.Save<DummySerializable>(null);
                AssertFile.Exists(false, this.TypeFiles.File);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.TypeFiles.Backup);
                }
            }
            else
            {
                Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(null));
            }
        }

        [Test]
        public void SaveNullName()
        {
            this.Repository.Save(this.NamedFiles.File, this.dummy);
            AssertFile.Exists(true, this.NamedFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.NamedFiles.Backup);
            }

            if ((this.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                this.Repository.Save<DummySerializable>(this.NamedFiles.File, null);
                AssertFile.Exists(false, this.NamedFiles.File);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.NamedFiles.Backup);
                }
            }
            else
            {
                Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(this.NamedFiles.File, null));
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
            this.Repository.Save(this.NamedFiles.File, list);
            AssertFile.Exists(true, this.NamedFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.NamedFiles.Backup);
            }

            var read = this.Read<List<DummySerializable>>(this.NamedFiles.File);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(this.dummy, read);

            list.RemoveAt(1);
            this.Repository.Save(this.NamedFiles.File, list);
            AssertFile.Exists(true, this.NamedFiles.File);
            read = this.Read<List<DummySerializable>>(this.NamedFiles.File);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(this.dummy, read);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteType(bool deleteBakups)
        {
            var dataRepository = this.Repository as IDataRepository;
            if (dataRepository == null)
            {
                return;
            }

            this.TypeFiles.File.CreatePlaceHolder();
            this.TypeFiles.SoftDelete.CreatePlaceHolder();
            if (this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreatePlaceHolder();
                AssertFile.Exists(true, this.TypeFiles.Backup);
            }

            AssertFile.Exists(true, this.TypeFiles.File);
            AssertFile.Exists(true, this.TypeFiles.SoftDelete);

            dataRepository.Delete<DummySerializable>(deleteBakups);
            AssertFile.Exists(false, this.TypeFiles.File);
            AssertFile.Exists(false, this.TypeFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(!deleteBakups, this.TypeFiles.Backup);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteName(bool deleteBakups)
        {
            var dataRepository = this.Repository as IDataRepository;
            if (dataRepository == null)
            {
                return;
            }

            this.NamedFiles.File.CreatePlaceHolder();
            this.NamedFiles.SoftDelete.CreatePlaceHolder();
            if (this.IsBackingUp)
            {
                this.NamedFiles.Backup.CreatePlaceHolder();
                AssertFile.Exists(true, this.NamedFiles.Backup);
            }

            AssertFile.Exists(true, this.NamedFiles.File);
            AssertFile.Exists(true, this.NamedFiles.SoftDelete);

            dataRepository.Delete(this.NamedFiles.File, deleteBakups);
            AssertFile.Exists(false, this.NamedFiles.File);
            AssertFile.Exists(false, this.NamedFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(!deleteBakups, this.NamedFiles.Backup);
            }
        }

        [Test]
        public void DeleteBackupsType()
        {
            this.TypeFiles.SoftDelete.CreatePlaceHolder();
            AssertFile.Exists(true, this.TypeFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreatePlaceHolder();
                AssertFile.Exists(true, this.TypeFiles.Backup);
            }

            this.Repository.DeleteBackups<DummySerializable>();
            AssertFile.Exists(false, this.TypeFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.TypeFiles.Backup);
            }
        }

        [Test]
        public void DeleteBackupsName()
        {
            this.NamedFiles.SoftDelete.CreatePlaceHolder();
            AssertFile.Exists(true, this.NamedFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                this.NamedFiles.Backup.CreatePlaceHolder();
                AssertFile.Exists(true, this.NamedFiles.Backup);
            }

            this.Repository.DeleteBackups(this.NamedFiles.File);
            AssertFile.Exists(false, this.NamedFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.NamedFiles.Backup);
            }
        }

        [Test]
        public void SaveType()
        {
            AssertFile.Exists(false, this.TypeFiles.File);
            this.Repository.Save(this.dummy);
            AssertFile.Exists(true, this.TypeFiles.File);
            AssertFile.Exists(false, this.TypeFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.TypeFiles.Backup);
            }

            this.dummy.Value++;
            this.Repository.Save(this.dummy);
            AssertFile.Exists(true, this.TypeFiles.File);
            AssertFile.Exists(false, this.TypeFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.TypeFiles.Backup);
            }

            var read = this.Read<DummySerializable>(this.TypeFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void SaveFileName()
        {
            AssertFile.Exists(false, this.NamedFiles.File);
            this.Repository.Save(this.NamedFiles.File, this.dummy);
            AssertFile.Exists(true, this.NamedFiles.File);
            AssertFile.Exists(false, this.NamedFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.NamedFiles.Backup);
            }

            this.dummy.Value++;
            this.Repository.Save(this.NamedFiles.File, this.dummy);
            AssertFile.Exists(true, this.NamedFiles.File);
            AssertFile.Exists(false, this.NamedFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.NamedFiles.Backup);
            }

            var read = this.Read<DummySerializable>(this.NamedFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void SaveTypeNull()
        {
            this.Repository.Save(this.dummy);
            AssertFile.Exists(true, this.TypeFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.TypeFiles.Backup);
            }

            if ((this.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                this.Repository.Save<DummySerializable>(null);
                AssertFile.Exists(false, this.TypeFiles.File);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.TypeFiles.Backup);
                }
            }
            else
            {
                Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(null));
            }
        }

        [Test]
        public void SaveFileNameNull()
        {
            this.Repository.Save(this.NamedFiles.File, this.dummy);
            AssertFile.Exists(true, this.NamedFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.NamedFiles.Backup);
            }

            if ((this.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                this.Repository.Save<DummySerializable>(this.NamedFiles.File, null);
                AssertFile.Exists(false, this.NamedFiles.File);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.NamedFiles.Backup);
                }
            }
            else
            {
                Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(this.NamedFiles.File, null));
            }
        }

        [Test]
        public async Task SaveTypeAsync()
        {
            await this.Repository.SaveAsync(this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.TypeFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.TypeFiles.Backup);
            }

            AssertFile.Exists(false, this.TypeFiles.SoftDelete);

            await this.Repository.SaveAsync(this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.TypeFiles.File);
            AssertFile.Exists(false, this.TypeFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.TypeFiles.Backup);
            }

            var read = this.Read<DummySerializable>(this.TypeFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public async Task SaveFileNameAsync()
        {
            await this.Repository.SaveAsync(this.NamedFiles.File, this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.NamedFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.NamedFiles.Backup);
            }

            AssertFile.Exists(false, this.NamedFiles.SoftDelete);

            await this.Repository.SaveAsync(this.NamedFiles.File, this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.NamedFiles.File);
            AssertFile.Exists(false, this.NamedFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(true, this.NamedFiles.Backup);
            }

            var read = this.Read<DummySerializable>(this.NamedFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public async Task SaveFileNameAsyncNull()
        {
            await this.Repository.SaveAsync(this.NamedFiles.File, this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.NamedFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.NamedFiles.Backup);
            }

            if ((this.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                await this.Repository.SaveAsync<DummySerializable>(this.NamedFiles.File, null).ConfigureAwait(false);
                AssertFile.Exists(false, this.NamedFiles.File);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.NamedFiles.Backup);
                }
            }
            else
            {
                Assert.ThrowsAsync<ArgumentNullException>(() => this.Repository.SaveAsync<DummySerializable>(this.NamedFiles.File, null));
            }
        }

        [Test]
        public async Task SaveTypeAsyncNull()
        {
            await this.Repository.SaveAsync(this.dummy).ConfigureAwait(false);
            AssertFile.Exists(true, this.TypeFiles.File);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.TypeFiles.Backup);
            }

            if ((this.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                await this.Repository.SaveAsync<DummySerializable>(this.TypeFiles.File, null).ConfigureAwait(false);
                AssertFile.Exists(false, this.TypeFiles.File);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(true, this.TypeFiles.Backup);
                }
            }
            else
            {
                Assert.ThrowsAsync<ArgumentNullException>(() => this.Repository.SaveAsync<DummySerializable>(null));
            }
        }

        [Test]
        public void SaveTypeCaches()
        {
            this.Repository.Save(this.dummy);
            var read = this.Repository.Read<DummySerializable>();
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy, read);
                Assert.AreNotSame(this.dummy, read);
            }

            this.dummy.Value++;
            this.Repository.Save(this.dummy);
            read = this.Read<DummySerializable>(this.TypeFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
        }

        [Test]
        public void SaveFileNameCaches()
        {
            this.Repository.Save(this.NamedFiles.File, this.dummy);
            var read = this.Repository.Read<DummySerializable>(this.NamedFiles.File);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy, read);
                Assert.AreNotSame(this.dummy, read);
            }

            this.dummy.Value++;
            this.Repository.Save(this.NamedFiles.File, this.dummy);
            read = this.Read<DummySerializable>(this.NamedFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveTypeAsyncCaches()
        {
            await this.Repository.SaveAsync(this.dummy).ConfigureAwait(false);
            var read = await this.Repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy, read);
                Assert.AreNotSame(this.dummy, read);
            }

            this.dummy.Value++;
            await this.Repository.SaveAsync(this.dummy).ConfigureAwait(false);
            read = this.Read<DummySerializable>(this.TypeFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
        }

        [Test]
        public async Task SaveFileNameAsyncCaches()
        {
            await this.Repository.SaveAsync(this.NamedFiles.File, this.dummy).ConfigureAwait(false);
            var read = await this.Repository.ReadAsync<DummySerializable>(this.NamedFiles.File).ConfigureAwait(false);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy, read);
                Assert.AreNotSame(this.dummy, read);
            }

            this.dummy.Value++;
            await this.Repository.SaveAsync(this.NamedFiles.File, this.dummy).ConfigureAwait(false);
            read = this.Read<DummySerializable>(this.NamedFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
        }

        [Test]
        public void SaveTypeThreeTimes()
        {
            this.Repository.Save(this.dummy);
            var read = this.Repository.Read<DummySerializable>();
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy, read);
                Assert.AreNotSame(this.dummy, read);
            }

            read = this.Read<DummySerializable>(this.TypeFiles.File);
            Assert.AreEqual(this.dummy, read);
            for (var i = 2; i < 3; i++)
            {
                this.dummy.Value++;
                this.Repository.Save(this.dummy);
                read = this.Repository.Read<DummySerializable>();
                if (this.Repository is ISingletonRepository)
                {
                    Assert.AreSame(this.dummy, read);
                }
                else
                {
                    Assert.AreEqual(this.dummy, read);
                    Assert.AreNotSame(this.dummy, read);
                }

                read = this.Read<DummySerializable>(this.TypeFiles.File);
                Assert.AreEqual(this.dummy, read);
                if (this.IsBackingUp)
                {
                    read = this.Read<DummySerializable>(this.TypeFiles.Backup);
                    Assert.AreEqual(this.dummy.Value - 1, read.Value);
                }
            }
        }

        [Test]
        public void SaveFileNameThreeTimes()
        {
            this.Repository.Save(this.NamedFiles.File, this.dummy);
            var read = this.Repository.Read<DummySerializable>(this.NamedFiles.File);
            if (this.Repository is ISingletonRepository)
            {
                Assert.AreSame(this.dummy, read);
            }
            else
            {
                Assert.AreEqual(this.dummy, read);
                Assert.AreNotSame(this.dummy, read);
            }

            read = this.Read<DummySerializable>(this.NamedFiles.File);
            Assert.AreEqual(this.dummy, read);
            for (var i = 2; i < 3; i++)
            {
                this.dummy.Value++;
                this.Repository.Save(this.NamedFiles.File, this.dummy);
                read = this.Repository.Read<DummySerializable>(this.NamedFiles.File);
                if (this.Repository is ISingletonRepository)
                {
                    Assert.AreSame(this.dummy, read);
                }
                else
                {
                    Assert.AreEqual(this.dummy, read);
                    Assert.AreNotSame(this.dummy, read);
                }

                read = this.Read<DummySerializable>(this.NamedFiles.File);
                Assert.AreEqual(this.dummy, read);
                if (this.IsBackingUp)
                {
                    read = this.Read<DummySerializable>(this.NamedFiles.Backup);
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
            if (this.Settings.IsTrackingDirty)
            {
                Assert.IsTrue(this.Repository.IsDirty(this.NamedFiles.File, this.dummy));

                this.Repository.Save(this.NamedFiles.File, this.dummy);
                Assert.IsFalse(this.Repository.IsDirty(this.NamedFiles.File, this.dummy));

                this.dummy.Value++;
                Assert.IsTrue(this.Repository.IsDirty(this.NamedFiles.File, this.dummy));
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => this.Repository.IsDirty(this.NamedFiles.File, this.dummy));
            }
        }

        [Test]
        public void CanRenameTypeHappyPath()
        {
            this.TypeFiles.File.CreatePlaceHolder();
            if (this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreatePlaceHolder();
            }

            Assert.IsTrue(this.Repository.CanRename<DummySerializable>("NewName"));
        }

        [Test]
        public void CanRenameFileNameHappyPath()
        {
            this.NamedFiles.File.CreatePlaceHolder();
            if (this.IsBackingUp)
            {
                this.NamedFiles.Backup.CreatePlaceHolder();
            }

            Assert.IsTrue(this.Repository.CanRename(this.NamedFiles.File, "NewName"));
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanRenameTypeWouldOverwrite(bool fileNewNameExists, bool backupNewNameExists)
        {
            this.TypeFiles.File.CreatePlaceHolder();
            if (fileNewNameExists)
            {
                this.TypeFiles.WithNewName.CreatePlaceHolder();
                Assert.AreEqual(false, this.Repository.CanRename<DummySerializable>("NewName"));
            }

            if (backupNewNameExists)
            {
                if (!this.IsBackingUp)
                {
                    return;
                }

                this.TypeFiles.BackupNewName.CreatePlaceHolder();
                this.TypeFiles.Backup.CreatePlaceHolder();
                this.TypeFiles.BackupNewName.CreatePlaceHolder();
                Assert.AreEqual(false, this.Repository.CanRename<DummySerializable>("NewName"));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanRenameNameWouldOverwrite(bool fileNewNameExists, bool backupNewNameExists)
        {
            this.NamedFiles.File.CreatePlaceHolder();
            if (fileNewNameExists)
            {
                this.NamedFiles.WithNewName.CreatePlaceHolder();
                Assert.AreEqual(false, this.Repository.CanRename(this.NamedFiles.File, "NewName"));
            }

            if (backupNewNameExists)
            {
                if (!this.IsBackingUp)
                {
                    return;
                }

                this.NamedFiles.BackupNewName.CreatePlaceHolder();
                this.NamedFiles.Backup.CreatePlaceHolder();
                this.NamedFiles.BackupNewName.CreatePlaceHolder();
                Assert.AreEqual(false, this.Repository.CanRename(this.NamedFiles.File, "NewName"));
            }
        }

        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void RenameType(bool hasBackup, bool hasSoft)
        {
            this.TypeFiles.File.CreatePlaceHolder();
            if (hasBackup && this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreatePlaceHolder();
            }

            if (hasSoft)
            {
                this.TypeFiles.SoftDelete.CreatePlaceHolder();
            }

            this.Repository.Rename<DummySerializable>("NewName", false);
            AssertFile.Exists(true, this.TypeFiles.WithNewName);
            if (hasBackup && this.IsBackingUp)
            {
                AssertFile.Exists(false, this.TypeFiles.Backup);
                AssertFile.Exists(true, this.TypeFiles.BackupNewName);
            }

            if (hasSoft)
            {
                AssertFile.Exists(false, this.TypeFiles.SoftDelete);
                AssertFile.Exists(true, this.TypeFiles.SoftDeleteNewName);
            }
        }

        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void RenameFileName(bool hasBackup, bool hasSoft)
        {
            this.NamedFiles.File.CreatePlaceHolder();
            if (hasBackup && this.IsBackingUp)
            {
                this.NamedFiles.Backup.CreatePlaceHolder();
            }

            if (hasSoft)
            {
                this.NamedFiles.SoftDelete.CreatePlaceHolder();
            }

            this.Repository.Rename(this.NamedFiles.File, "NewName", false);
            AssertFile.Exists(true, this.NamedFiles.WithNewName);
            if (hasBackup && this.IsBackingUp)
            {
                AssertFile.Exists(false, this.NamedFiles.Backup);
                AssertFile.Exists(true, this.NamedFiles.BackupNewName);
            }

            if (hasSoft)
            {
                AssertFile.Exists(false, this.NamedFiles.SoftDelete);
                AssertFile.Exists(true, this.NamedFiles.SoftDeleteNewName);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RenameTypeOverwrite(bool owerWrite)
        {
            this.TypeFiles.File.CreatePlaceHolder("a");
            this.TypeFiles.SoftDelete.CreatePlaceHolder("c");

            this.TypeFiles.WithNewName.CreatePlaceHolder("aa");
            this.TypeFiles.SoftDeleteNewName.CreatePlaceHolder("cc");
            if (this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreatePlaceHolder("b");
                this.TypeFiles.BackupNewName.CreatePlaceHolder("bb");
            }

            if (owerWrite)
            {
                this.Repository.Rename<DummySerializable>("NewName", true);
                AssertFile.Exists(false, this.TypeFiles.File);
                AssertFile.Exists(false, this.TypeFiles.SoftDelete);

                Assert.AreEqual("a", this.TypeFiles.WithNewName.ReadAllText());
                Assert.AreEqual("c", this.TypeFiles.SoftDeleteNewName.ReadAllText());
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(false, this.TypeFiles.Backup);
                    Assert.AreEqual("b", this.TypeFiles.BackupNewName.ReadAllText());
                }
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => this.Repository.Rename<DummySerializable>("NewName", false));
                Assert.AreEqual("a", this.TypeFiles.File.ReadAllText());
                Assert.AreEqual("c", this.TypeFiles.SoftDelete.ReadAllText());

                Assert.AreEqual("aa", this.TypeFiles.WithNewName.ReadAllText());
                Assert.AreEqual("cc", this.TypeFiles.SoftDeleteNewName.ReadAllText());
                if (this.IsBackingUp)
                {
                    Assert.AreEqual("b", this.TypeFiles.Backup.ReadAllText());
                    Assert.AreEqual("bb", this.TypeFiles.BackupNewName.ReadAllText());
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RenameFileNameOverwrite(bool owerWrite)
        {
            this.NamedFiles.File.CreatePlaceHolder("a");
            this.NamedFiles.SoftDelete.CreatePlaceHolder("c");

            this.NamedFiles.WithNewName.CreatePlaceHolder("aa");
            this.NamedFiles.SoftDeleteNewName.CreatePlaceHolder("cc");
            if (this.IsBackingUp)
            {
                this.NamedFiles.Backup.CreatePlaceHolder("b");
                this.NamedFiles.BackupNewName.CreatePlaceHolder("bb");
            }

            if (owerWrite)
            {
                this.Repository.Rename(this.NamedFiles.File, "NewName", true);
                AssertFile.Exists(false, this.NamedFiles.File);
                AssertFile.Exists(false, this.NamedFiles.SoftDelete);

                Assert.AreEqual("a", this.NamedFiles.WithNewName.ReadAllText());
                Assert.AreEqual("c", this.NamedFiles.SoftDeleteNewName.ReadAllText());
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(false, this.NamedFiles.Backup);
                    Assert.AreEqual("b", this.NamedFiles.BackupNewName.ReadAllText());
                }
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => this.Repository.Rename(this.NamedFiles.File, "NewName", false));
                Assert.AreEqual("a", this.NamedFiles.File.ReadAllText());
                Assert.AreEqual("c", this.NamedFiles.SoftDelete.ReadAllText());

                Assert.AreEqual("aa", this.NamedFiles.WithNewName.ReadAllText());
                Assert.AreEqual("cc", this.NamedFiles.SoftDeleteNewName.ReadAllText());
                if (this.IsBackingUp)
                {
                    Assert.AreEqual("b", this.NamedFiles.Backup.ReadAllText());
                    Assert.AreEqual("bb", this.NamedFiles.BackupNewName.ReadAllText());
                }
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