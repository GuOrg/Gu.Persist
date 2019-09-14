#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using Gu.Persist.Core;

    using NUnit.Framework;

    public abstract class RepositoryTests
    {
        private readonly DummySerializable dummy = new DummySerializable(1);

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        protected RepositoryTests()
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);

            // Default directory is created in %APPDATA%/AppName
            // overriding it here in tests.
            typeof(Directories).GetField("default", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly).SetValue(null, directory);
            this.Directory = directory;

            // Just a check to be sure test is not producing files outside %TEMP%
            // ReSharper disable once VirtualMemberCallInConstructor
#pragma warning disable CA2214 // Do not call overridable methods in constructors
            Assert.AreEqual(true, this.CreateRepository().Settings.Directory.StartsWith(Directories.TempDirectory.FullName, StringComparison.InvariantCulture));
#pragma warning restore CA2214 // Do not call overridable methods in constructors
        }

        public RepositorySettings Settings => (RepositorySettings)this.Repository?.Settings;

        public BackupSettings BackupSettings => this.Settings?.BackupSettings;

        public bool IsBackingUp => this.BackupSettings != null;

        public IRepository Repository { get; private set; }

        public DirectoryInfo Directory { get; }

        public Files NamedFiles { get; private set; }

        public Files TypeFiles { get; private set; }

        public FileInfo RepoSettingFile { get; private set; }

        [SetUp]
        public void SetUp()
        {
            this.Directory.DeleteIfExists(recursive: true);
            this.Repository = this.CreateRepository();
            this.Repository.ClearCache();
            this.NamedFiles = new Files(this.GetType().Name, this.Settings);
            this.TypeFiles = new Files(this.dummy.GetType().Name, this.Settings);
            this.RepoSettingFile = this.Directory.CreateFileInfoInDirectory(string.Concat(this.Settings.GetType().Name, this.Settings.Extension));
        }

        [TearDown]
        public void TearDown()
        {
            this.Directory.DeleteIfExists(true);
        }

        [Test]
        public void ReadFileInfo()
        {
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(fileInfo, dummy);
            var read = repository.Read<DummySerializable>(fileInfo);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void ReadName()
        {
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(fileInfo, dummy);
            var read = repository.Read<DummySerializable>(fileInfo.Name);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void ReadFullName()
        {
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(fileInfo, dummy);
            var read = repository.Read<DummySerializable>(fileInfo.FullName);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void ReadGeneric()
        {
            var repository = this.CreateRepository();
            var dummy = new DummySerializable(1);
            var fileInfo = repository.GetGenericTestFileInfo(dummy);
            this.Save(fileInfo, dummy);
            var read = repository.Read<DummySerializable>();
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task ReadAsyncFileInfo()
        {
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(fileInfo, dummy);
            var read = await repository.ReadAsync<DummySerializable>(fileInfo).ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task ReadAsyncName()
        {
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(fileInfo, dummy);
            var read = await repository.ReadAsync<DummySerializable>(fileInfo.Name).ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task ReadAsyncFullName()
        {
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(fileInfo, dummy);
            var read = await repository.ReadAsync<DummySerializable>(fileInfo.FullName).ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task ReadAsyncGeneric()
        {
            var repository = this.CreateRepository();
            var dummy = new DummySerializable(1);
            var fileInfo = repository.GetGenericTestFileInfo(dummy);
            this.Save(fileInfo, dummy);
            var read = await repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateFileInfo(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();

            if (exists)
            {
                this.Save(fileInfo, dummy);
            }

            var read = repository.ReadOrCreate(fileInfo, () => dummy);
            AssertFile.Exists(true, fileInfo);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateFullName(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();

            if (exists)
            {
                this.Save(fileInfo, dummy);
            }

            var read = repository.ReadOrCreate(fileInfo.FullName, () => dummy);
            AssertFile.Exists(true, fileInfo);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateName(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();

            if (exists)
            {
                this.Save(fileInfo, dummy);
            }

            var read = repository.ReadOrCreate(fileInfo.Name, () => dummy);
            AssertFile.Exists(true, fileInfo);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateGeneric(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetGenericTestFileInfo(dummy);

            if (exists)
            {
                this.Save(fileInfo, dummy);
            }

            var read = repository.ReadOrCreate(() => dummy);
            AssertFile.Exists(true, fileInfo);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [Test]
        public void ReadFileInfoCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            this.Save(fileInfo, dummy);
            var read1 = repository.Read<DummySerializable>(fileInfo);
            var read2 = repository.Read<DummySerializable>(fileInfo);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public void ReadFullNameCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            this.Save(fileInfo, dummy);
            var read1 = repository.Read<DummySerializable>(fileInfo.FullName);
            var read2 = repository.Read<DummySerializable>(fileInfo.FullName);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public void ReadNameCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            this.Save(fileInfo, dummy);
            var read1 = repository.Read<DummySerializable>(fileInfo.Name);
            var read2 = repository.Read<DummySerializable>(fileInfo.Name);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public void ReadGenericCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetGenericTestFileInfo(dummy);
            this.Save(fileInfo, dummy);
            var read1 = repository.Read<DummySerializable>();
            var read2 = repository.Read<DummySerializable>();
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public async Task ReadAsyncFileInfoCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            this.Save(fileInfo, dummy);
            var read1 = await repository.ReadAsync<DummySerializable>(fileInfo).ConfigureAwait(false);
            var read2 = await repository.ReadAsync<DummySerializable>(fileInfo).ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public async Task ReadAsyncFullNameCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            this.Save(fileInfo, dummy);
            var read1 = await repository.ReadAsync<DummySerializable>(fileInfo.FullName).ConfigureAwait(false);
            var read2 = await repository.ReadAsync<DummySerializable>(fileInfo.FullName).ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public async Task ReadAsyncNameCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            this.Save(fileInfo, dummy);
            var read1 = await repository.ReadAsync<DummySerializable>(fileInfo.Name).ConfigureAwait(false);
            var read2 = await repository.ReadAsync<DummySerializable>(fileInfo.Name).ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public async Task ReadAsyncGenericCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetGenericTestFileInfo(dummy);
            this.Save(fileInfo, dummy);
            var read1 = await repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
            var read2 = await repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public void SaveThenReadFileInfo()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            AssertFile.Exists(false, fileInfo);
            repository.Save(fileInfo, dummy);
            AssertFile.Exists(true, fileInfo);

            var read = repository.Read<DummySerializable>(fileInfo);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(dummy, read);
            }
            else
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
        }

        [Test]
        public void SaveThenReadFullName()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            AssertFile.Exists(false, fileInfo);
            repository.Save(fileInfo.FullName, dummy);
            AssertFile.Exists(true, fileInfo);

            var read = repository.Read<DummySerializable>(fileInfo.FullName);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(dummy, read);
            }
            else
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
        }

        [Test]
        public void SaveThenReadName()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            AssertFile.Exists(false, fileInfo);
            repository.Save(fileInfo.Name, dummy);
            AssertFile.Exists(true, fileInfo);

            var read = repository.Read<DummySerializable>(fileInfo.Name);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(dummy, read);
            }
            else
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
        }

        [Test]
        public void SaveThenReadGeneric()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetGenericTestFileInfo(dummy);
            AssertFile.Exists(false, fileInfo);
            repository.Save(dummy);
            AssertFile.Exists(true, fileInfo);

            var read = repository.Read<DummySerializable>();
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(dummy, read);
            }
            else
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
        }

        [Test]
        public async Task SaveThenReadAsyncFileInfo()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            AssertFile.Exists(false, fileInfo);
            await repository.SaveAsync(fileInfo, dummy).ConfigureAwait(false);
            AssertFile.Exists(true, fileInfo);

            var read = await repository.ReadAsync<DummySerializable>(fileInfo).ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(dummy, read);
            }
            else
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
        }

        [Test]
        public async Task SaveThenReadAsyncFullName()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            AssertFile.Exists(false, fileInfo);
            await repository.SaveAsync(fileInfo.FullName, dummy).ConfigureAwait(false);
            AssertFile.Exists(true, fileInfo);

            var read = await repository.ReadAsync<DummySerializable>(fileInfo.FullName).ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(dummy, read);
            }
            else
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
        }

        [Test]
        public async Task SaveThenReadAsyncName()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetTestFileInfo();
            AssertFile.Exists(false, fileInfo);
            await repository.SaveAsync(fileInfo.Name, dummy).ConfigureAwait(false);
            AssertFile.Exists(true, fileInfo);

            var read = await repository.ReadAsync<DummySerializable>(fileInfo.Name).ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(dummy, read);
            }
            else
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
        }

        [Test]
        public async Task SaveThenReadAsyncGeneric()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var fileInfo = repository.GetGenericTestFileInfo(dummy);
            AssertFile.Exists(false, fileInfo);
            await repository.SaveAsync(dummy).ConfigureAwait(false);
            AssertFile.Exists(true, fileInfo);

            var read = await repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(dummy, read);
            }
            else
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
        }

        [Test]
        public void SaveNullGeneric()
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
                _ = Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(null));
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
                _ = Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(this.NamedFiles.File, null));
            }
        }

        [Test]
        public void SaveLongListThenShortListFile()
        {
            var list = new List<DummySerializable>
            {
                this.dummy,
                new DummySerializable(2),
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
        public void DeleteType(bool deleteBackups)
        {
            if (this.Repository is IDataRepository dataRepository)
            {
                this.TypeFiles.File.CreatePlaceHolder();
                this.TypeFiles.SoftDelete.CreatePlaceHolder();
                if (this.IsBackingUp)
                {
                    this.TypeFiles.Backup.CreatePlaceHolder();
                    AssertFile.Exists(true, this.TypeFiles.Backup);
                }

                AssertFile.Exists(true, this.TypeFiles.File);
                AssertFile.Exists(true, this.TypeFiles.SoftDelete);

                dataRepository.Delete<DummySerializable>(deleteBackups);
                AssertFile.Exists(false, this.TypeFiles.File);
                AssertFile.Exists(false, this.TypeFiles.SoftDelete);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(!deleteBackups, this.TypeFiles.Backup);
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteName(bool deleteBackups)
        {
            if (this.Repository is IDataRepository dataRepository)
            {
                this.NamedFiles.File.CreatePlaceHolder();
                this.NamedFiles.SoftDelete.CreatePlaceHolder();
                if (this.IsBackingUp)
                {
                    this.NamedFiles.Backup.CreatePlaceHolder();
                    AssertFile.Exists(true, this.NamedFiles.Backup);
                }

                AssertFile.Exists(true, this.NamedFiles.File);
                AssertFile.Exists(true, this.NamedFiles.SoftDelete);

                dataRepository.Delete(this.NamedFiles.File, deleteBackups);
                AssertFile.Exists(false, this.NamedFiles.File);
                AssertFile.Exists(false, this.NamedFiles.SoftDelete);
                if (this.IsBackingUp)
                {
                    AssertFile.Exists(!deleteBackups, this.NamedFiles.Backup);
                }
            }
        }

        [Test]
        public void DeleteBackupsGeneric()
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
        public void SaveGeneric()
        {
            AssertFile.Exists(false, this.TypeFiles.File);
            this.Repository.Save(this.dummy);
            AssertFile.Exists(true, this.TypeFiles.File);
            AssertFile.Exists(false, this.TypeFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                AssertFile.Exists(false, this.TypeFiles.Backup);
            }

            var read = this.Read<DummySerializable>(this.TypeFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void SaveTypeTwice()
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

            var read = this.Read<DummySerializable>(this.NamedFiles.File);
            Assert.AreEqual(this.dummy.Value, read.Value);
            Assert.AreNotSame(this.dummy, read);
        }

        [Test]
        public void SaveFileNameTwice()
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
                _ = Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(null));
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
                _ = Assert.Throws<ArgumentNullException>(() => this.Repository.Save<DummySerializable>(this.NamedFiles.File, null));
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
                _ = Assert.ThrowsAsync<ArgumentNullException>(() => this.Repository.SaveAsync<DummySerializable>(this.NamedFiles.File, null));
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
                _ = Assert.ThrowsAsync<ArgumentNullException>(() => this.Repository.SaveAsync<DummySerializable>(null));
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
        public void IsDirtyGeneric()
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
                var exception = Assert.Throws<InvalidOperationException>(() => this.Repository.IsDirty(this.dummy));
                Assert.AreEqual("This repository is not tracking dirty.", exception.Message);
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
                var exception = Assert.Throws<InvalidOperationException>(() => this.Repository.IsDirty(this.NamedFiles.File, this.dummy));
                Assert.AreEqual("This repository is not tracking dirty.", exception.Message);
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
        public void CanRenameTypeWhenIllegalName()
        {
            this.TypeFiles.File.CreatePlaceHolder();
            if (this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreatePlaceHolder();
            }

            var exception = Assert.Throws<ArgumentException>(() => this.Repository.CanRename<DummySerializable>("NewName<>"));
            Assert.AreEqual("newName\r\nParameter name: NewName<> is not a valid filename. Contains: {'<', '>'}.", exception.Message);
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
        public void RenameTypeOverwrite(bool overWrite)
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

            if (overWrite)
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
                _ = Assert.Throws<InvalidOperationException>(() => this.Repository.Rename<DummySerializable>("NewName", false));
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
        public void RenameFileNameOverwrite(bool overWrite)
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

            if (overWrite)
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
                _ = Assert.Throws<InvalidOperationException>(() => this.Repository.Rename(this.NamedFiles.File, "NewName", false));
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

        protected abstract IRepository CreateRepository();

        protected abstract void Save<T>(FileInfo file, T item);

        protected abstract T Read<T>(FileInfo file);
    }
}