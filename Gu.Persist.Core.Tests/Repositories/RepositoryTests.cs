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

        public RepositorySettings Settings => (RepositorySettings)Repository?.Settings;

        public BackupSettings BackupSettings => this.Settings?.BackupSettings;

        public IRepository Repository { get; private set; }

        public DirectoryInfo Directory { get; }

        public System.IO.FileInfo RepoSettingFile { get; private set; }

        [SetUp]
        public void SetUp()
        {
            this.Directory.DeleteIfExists(recursive: true);
            Repository = this.CreateRepository();
            Repository.ClearCache();
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
                var softDelete = file.SoftDeleteFile();
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
                var softDelete = file.SoftDeleteFile();
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
                var softDelete = file.SoftDeleteFile();
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
                var softDelete = file.SoftDeleteFile();
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
        public void CanRenameFileInfoWouldOverwrite(bool fileNewNameExists, bool backupNewNameExists)
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings);
            file.CreateFileOnDisk();
            if (fileNewNameExists)
            {
                file.WithNewName("NewName", repository.Settings).CreateFileOnDisk();
                Assert.AreEqual(false, repository.CanRename(file, "NewName"));
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

        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void RenameFileInfo(bool hasBackup, bool hasSoft)
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings);
            file.CreateFileOnDisk();
            if (hasBackup &&
                repository.Settings.BackupSettings != null)
            {
                BackupFile.CreateFor(file, repository.Settings.BackupSettings).CreateFileOnDisk();
            }

            if (hasSoft)
            {
                file.SoftDeleteFile().CreateFileOnDisk();
            }

            repository.Rename(file, "NewName", false);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, file.WithNewName("NewName", repository.Settings));
            if (hasBackup &&
                repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, BackupFile.CreateFor(file, repository.Settings.BackupSettings));
                AssertFile.Exists(true, BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings));
            }

            if (hasSoft)
            {
                AssertFile.Exists(false, file.SoftDeleteFile());
                AssertFile.Exists(true, file.WithNewName("NewName", repository.Settings).SoftDeleteFile());
            }
        }

        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void RenameGeneric(bool hasBackup, bool hasSoft)
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings, nameof(DummySerializable));
            file.CreateFileOnDisk();
            if (hasBackup &&
                repository.Settings.BackupSettings != null)
            {
                BackupFile.CreateFor(file, repository.Settings.BackupSettings).CreateFileOnDisk();
            }

            if (hasSoft)
            {
                file.SoftDeleteFile().CreateFileOnDisk();
            }

            repository.Rename<DummySerializable>("NewName", false);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, file.WithNewName("NewName", repository.Settings));
            if (hasBackup &&
                repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, BackupFile.CreateFor(file, repository.Settings.BackupSettings));
                AssertFile.Exists(true, BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings));
            }

            if (hasSoft)
            {
                AssertFile.Exists(false, file.SoftDeleteFile());
                AssertFile.Exists(true, file.WithNewName("NewName", repository.Settings).SoftDeleteFile());
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RenameFileNameOverwrite(bool overWrite)
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings);
            var softDelete = file.SoftDeleteFile();
            file.CreateFileOnDisk("file");
            softDelete.CreateFileOnDisk("file soft delete");

            file.WithNewName("NewName", repository.Settings).CreateFileOnDisk("existing new name");
            file.WithNewName("NewName", repository.Settings).SoftDeleteFile().CreateFileOnDisk("existing soft delete new name");
            if (repository.Settings.BackupSettings != null)
            {
                BackupFile.CreateFor(file, repository.Settings.BackupSettings).CreateFileOnDisk("backup");
                BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings).CreateFileOnDisk("existing backup new name");
            }

            if (overWrite)
            {
                repository.Rename(file, "NewName", true);
                AssertFile.Exists(false, file);
                AssertFile.Exists(false, softDelete);

                Assert.AreEqual("file", file.WithNewName("NewName", repository.Settings).ReadAllText());
                Assert.AreEqual("file soft delete", file.WithNewName("NewName", repository.Settings).SoftDeleteFile().ReadAllText());
                if (repository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(false, BackupFile.CreateFor(file, repository.Settings.BackupSettings));
                    Assert.AreEqual("backup", BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings).ReadAllText());
                }
            }
            else
            {
                _ = Assert.Throws<InvalidOperationException>(() => repository.Rename(file, "NewName", false));
                Assert.AreEqual("file", file.ReadAllText());
                Assert.AreEqual("file soft delete", softDelete.ReadAllText());

                Assert.AreEqual("existing new name", file.WithNewName("NewName", repository.Settings).ReadAllText());
                Assert.AreEqual("existing soft delete new name", file.WithNewName("NewName", repository.Settings).SoftDeleteFile().ReadAllText());
                if (repository.Settings.BackupSettings != null)
                {
                    Assert.AreEqual("backup", BackupFile.CreateFor(file, repository.Settings.BackupSettings).ReadAllText());
                    Assert.AreEqual("existing backup new name", BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings).ReadAllText());
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RenameGenericOverwrite(bool overWrite)
        {
            var repository = this.CreateRepository();
            var file = CreateTestFile(repository.Settings, nameof(DummySerializable));
            var softDelete = file.SoftDeleteFile();
            file.CreateFileOnDisk("file");
            softDelete.CreateFileOnDisk("file soft delete");

            file.WithNewName("NewName", repository.Settings).CreateFileOnDisk("existing new name");
            file.WithNewName("NewName", repository.Settings).SoftDeleteFile().CreateFileOnDisk("existing soft delete new name");
            if (repository.Settings.BackupSettings != null)
            {
                BackupFile.CreateFor(file, repository.Settings.BackupSettings).CreateFileOnDisk("backup");
                BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings).CreateFileOnDisk("existing backup new name");
            }

            if (overWrite)
            {
                repository.Rename<DummySerializable>("NewName", true);
                AssertFile.Exists(false, file);
                AssertFile.Exists(false, softDelete);

                Assert.AreEqual("file", file.WithNewName("NewName", repository.Settings).ReadAllText());
                Assert.AreEqual("file soft delete", file.WithNewName("NewName", repository.Settings).SoftDeleteFile().ReadAllText());
                if (repository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(false, BackupFile.CreateFor(file, repository.Settings.BackupSettings));
                    Assert.AreEqual("backup", BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings).ReadAllText());
                }
            }
            else
            {
                _ = Assert.Throws<InvalidOperationException>(() => repository.Rename<DummySerializable>("NewName", false));
                Assert.AreEqual("file", file.ReadAllText());
                Assert.AreEqual("file soft delete", softDelete.ReadAllText());

                Assert.AreEqual("existing new name", file.WithNewName("NewName", repository.Settings).ReadAllText());
                Assert.AreEqual("existing soft delete new name", file.WithNewName("NewName", repository.Settings).SoftDeleteFile().ReadAllText());
                if (repository.Settings.BackupSettings != null)
                {
                    Assert.AreEqual("backup", BackupFile.CreateFor(file, repository.Settings.BackupSettings).ReadAllText());
                    Assert.AreEqual("existing backup new name", BackupFile.CreateFor(file.WithNewName("NewName", repository.Settings), repository.Settings.BackupSettings).ReadAllText());
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