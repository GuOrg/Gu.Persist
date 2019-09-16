#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    public abstract partial class RepositoryTests
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

        public System.IO.FileInfo RepoSettingFile { get; private set; }

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

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteFileInfo(bool deleteBackups)
        {
            if (this.CreateRepository() is IDataRepository dataRepository)
            {
                var file = CreateTestFile(dataRepository.Settings);
                var softDelete = file.GetSoftDeleteFileFor();
                file.CreateFileOnDisk();
                softDelete.CreateFileOnDisk();
                if (dataRepository.Settings.BackupSettings != null)
                {
                    BackupFile.CreateFor(file, dataRepository.Settings.BackupSettings).CreateFileOnDisk();
                }

                AssertFile.Exists(true, file);
                AssertFile.Exists(true, softDelete);

                dataRepository.Delete(file, deleteBackups);
                AssertFile.Exists(false, file);
                AssertFile.Exists(false, softDelete);
                if (dataRepository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(!deleteBackups, BackupFile.CreateFor(file, dataRepository.Settings.BackupSettings));
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteFullFileName(bool deleteBackups)
        {
            if (this.CreateRepository() is IDataRepository dataRepository)
            {
                var file = CreateTestFile(dataRepository.Settings);
                var softDelete = file.GetSoftDeleteFileFor();
                file.CreateFileOnDisk();
                softDelete.CreateFileOnDisk();
                if (dataRepository.Settings.BackupSettings != null)
                {
                    BackupFile.CreateFor(file, dataRepository.Settings.BackupSettings).CreateFileOnDisk();
                }

                AssertFile.Exists(true, file);
                AssertFile.Exists(true, softDelete);

                dataRepository.Delete(file.FullName, deleteBackups);
                AssertFile.Exists(false, file);
                AssertFile.Exists(false, softDelete);
                if (dataRepository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(!deleteBackups, BackupFile.CreateFor(file, dataRepository.Settings.BackupSettings));
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteFileName(bool deleteBackups)
        {
            if (this.CreateRepository() is IDataRepository dataRepository)
            {
                var file = CreateTestFile(dataRepository.Settings);
                var softDelete = file.GetSoftDeleteFileFor();
                file.CreateFileOnDisk();
                softDelete.CreateFileOnDisk();
                if (dataRepository.Settings.BackupSettings != null)
                {
                    BackupFile.CreateFor(file, dataRepository.Settings.BackupSettings).CreateFileOnDisk();
                }

                AssertFile.Exists(true, file);
                AssertFile.Exists(true, softDelete);

                dataRepository.Delete(file.Name, deleteBackups);
                AssertFile.Exists(false, file);
                AssertFile.Exists(false, softDelete);
                if (dataRepository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(!deleteBackups, BackupFile.CreateFor(file, dataRepository.Settings.BackupSettings));
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteGeneric(bool deleteBackups)
        {
            if (this.CreateRepository() is IDataRepository dataRepository)
            {
                var file = CreateTestFile(dataRepository.Settings, nameof(DummySerializable));
                var softDelete = file.GetSoftDeleteFileFor();
                file.CreateFileOnDisk();
                softDelete.CreateFileOnDisk();
                if (dataRepository.Settings.BackupSettings != null)
                {
                    BackupFile.CreateFor(file, dataRepository.Settings.BackupSettings).CreateFileOnDisk();
                }

                AssertFile.Exists(true, file);
                AssertFile.Exists(true, softDelete);

                dataRepository.Delete<DummySerializable>(deleteBackups);
                AssertFile.Exists(false, file);
                AssertFile.Exists(false, softDelete);
                if (dataRepository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(!deleteBackups, BackupFile.CreateFor(file, dataRepository.Settings.BackupSettings));
                }
            }
        }

        [Test]
        public void CanRenameFileInfo()
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings);
            file.CreateFileOnDisk();
            if (repository.Settings.BackupSettings != null)
            {
                BackupFile.CreateFor(file, repository.Settings.BackupSettings).CreateFileOnDisk();
            }

            Assert.AreEqual(true, repository.CanRename(file, "NewName"));
        }

        [Test]
        public void CanRenameFullFileName()
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings);
            file.CreateFileOnDisk();
            if (repository.Settings.BackupSettings != null)
            {
                BackupFile.CreateFor(file, repository.Settings.BackupSettings).CreateFileOnDisk();
            }

            Assert.AreEqual(true, repository.CanRename(file.FullName, "NewName"));
        }

        [Test]
        public void CanRenameGeneric()
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings, nameof(DummySerializable));
            file.CreateFileOnDisk();
            if (repository.Settings.BackupSettings != null)
            {
                BackupFile.CreateFor(file, repository.Settings.BackupSettings).CreateFileOnDisk();
            }

            Assert.AreEqual(true, repository.CanRename<DummySerializable>("NewName"));
        }

        [Test]
        public void CanRenameGenericWhenIllegalName()
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings, nameof(DummySerializable));
            file.CreateFileOnDisk();
            if (repository.Settings.BackupSettings != null)
            {
                BackupFile.CreateFor(file, repository.Settings.BackupSettings).CreateFileOnDisk();
            }

            var exception = Assert.Throws<ArgumentException>(() => repository.CanRename<DummySerializable>("NewName<>"));
            Assert.AreEqual("Illegal characters in path.", exception.Message);
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanRenameGenericWouldOverwrite(bool fileNewNameExists, bool backupNewNameExists)
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings, nameof(DummySerializable));
            file.CreateFileOnDisk();
            if (fileNewNameExists)
            {
                file.WithNewName("NewName", repository.Settings).CreateFileOnDisk();
                Assert.AreEqual(false, repository.CanRename<DummySerializable>("NewName"));
            }

            if (backupNewNameExists &&
                repository.Settings.BackupSettings != null)
            {
                var backup = BackupFile.CreateFor(file, repository.Settings.BackupSettings);
                backup.CreateFileOnDisk();
                BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings).CreateFileOnDisk();
                Assert.AreEqual(false, repository.CanRename<DummySerializable>("NewName"));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanRenameNameWouldOverwrite(bool fileNewNameExists, bool backupNewNameExists)
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings);
            file.CreateFileOnDisk();
            if (fileNewNameExists)
            {
                file.WithNewName("NewName", repository.Settings).CreateFileOnDisk();
                Assert.AreEqual(false, this.Repository.CanRename(file, "NewName"));
            }

            if (backupNewNameExists &&
                repository.Settings.BackupSettings != null)
            {
                var backup = BackupFile.CreateFor(file, repository.Settings.BackupSettings);
                backup.CreateFileOnDisk();
                BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings).CreateFileOnDisk();
                Assert.AreEqual(false, repository.CanRename(file, "NewName"));
            }
        }

        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void RenameType(bool hasBackup, bool hasSoft)
        {
            this.TypeFiles.File.CreateFileOnDisk();
            if (hasBackup && this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreateFileOnDisk();
            }

            if (hasSoft)
            {
                this.TypeFiles.SoftDelete.CreateFileOnDisk();
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
            this.NamedFiles.File.CreateFileOnDisk();
            if (hasBackup && this.IsBackingUp)
            {
                this.NamedFiles.Backup.CreateFileOnDisk();
            }

            if (hasSoft)
            {
                this.NamedFiles.SoftDelete.CreateFileOnDisk();
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
            this.TypeFiles.File.CreateFileOnDisk("a");
            this.TypeFiles.SoftDelete.CreateFileOnDisk("c");

            this.TypeFiles.WithNewName.CreateFileOnDisk("aa");
            this.TypeFiles.SoftDeleteNewName.CreateFileOnDisk("cc");
            if (this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreateFileOnDisk("b");
                this.TypeFiles.BackupNewName.CreateFileOnDisk("bb");
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
            this.NamedFiles.File.CreateFileOnDisk("a");
            this.NamedFiles.SoftDelete.CreateFileOnDisk("c");

            this.NamedFiles.WithNewName.CreateFileOnDisk("aa");
            this.NamedFiles.SoftDeleteNewName.CreateFileOnDisk("cc");
            if (this.IsBackingUp)
            {
                this.NamedFiles.Backup.CreateFileOnDisk("b");
                this.NamedFiles.BackupNewName.CreateFileOnDisk("bb");
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

        protected static System.IO.FileInfo CreateTestFile(IRepositorySettings settings, [CallerMemberName] string name = null) => new System.IO.FileInfo(Path.Combine(settings.Directory, name + settings.Extension));

        protected abstract IRepository CreateRepository();

        protected abstract void Save<T>(System.IO.FileInfo file, T item);

        protected abstract T Read<T>(System.IO.FileInfo file);
    }
}