#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    using Gu.Persist.Core;

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
        public void DeleteType(bool deleteBackups)
        {
            if (this.Repository is IDataRepository dataRepository)
            {
                this.TypeFiles.File.CreateFileOnDisk();
                this.TypeFiles.SoftDelete.CreateFileOnDisk();
                if (this.IsBackingUp)
                {
                    this.TypeFiles.Backup.CreateFileOnDisk();
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
                this.NamedFiles.File.CreateFileOnDisk();
                this.NamedFiles.SoftDelete.CreateFileOnDisk();
                if (this.IsBackingUp)
                {
                    this.NamedFiles.Backup.CreateFileOnDisk();
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
            this.TypeFiles.SoftDelete.CreateFileOnDisk();
            AssertFile.Exists(true, this.TypeFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreateFileOnDisk();
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
            this.NamedFiles.SoftDelete.CreateFileOnDisk();
            AssertFile.Exists(true, this.NamedFiles.SoftDelete);
            if (this.IsBackingUp)
            {
                this.NamedFiles.Backup.CreateFileOnDisk();
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
            this.TypeFiles.File.CreateFileOnDisk();
            if (this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreateFileOnDisk();
            }

            Assert.IsTrue(this.Repository.CanRename<DummySerializable>("NewName"));
        }

        [Test]
        public void CanRenameTypeWhenIllegalName()
        {
            this.TypeFiles.File.CreateFileOnDisk();
            if (this.IsBackingUp)
            {
                this.TypeFiles.Backup.CreateFileOnDisk();
            }

            var exception = Assert.Throws<ArgumentException>(() => this.Repository.CanRename<DummySerializable>("NewName<>"));
            Assert.AreEqual("newName\r\nParameter name: NewName<> is not a valid filename. Contains: {'<', '>'}.", exception.Message);
        }

        [Test]
        public void CanRenameFileNameHappyPath()
        {
            this.NamedFiles.File.CreateFileOnDisk();
            if (this.IsBackingUp)
            {
                this.NamedFiles.Backup.CreateFileOnDisk();
            }

            Assert.IsTrue(this.Repository.CanRename(this.NamedFiles.File, "NewName"));
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanRenameTypeWouldOverwrite(bool fileNewNameExists, bool backupNewNameExists)
        {
            this.TypeFiles.File.CreateFileOnDisk();
            if (fileNewNameExists)
            {
                this.TypeFiles.WithNewName.CreateFileOnDisk();
                Assert.AreEqual(false, this.Repository.CanRename<DummySerializable>("NewName"));
            }

            if (backupNewNameExists)
            {
                if (!this.IsBackingUp)
                {
                    return;
                }

                this.TypeFiles.BackupNewName.CreateFileOnDisk();
                this.TypeFiles.Backup.CreateFileOnDisk();
                this.TypeFiles.BackupNewName.CreateFileOnDisk();
                Assert.AreEqual(false, this.Repository.CanRename<DummySerializable>("NewName"));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanRenameNameWouldOverwrite(bool fileNewNameExists, bool backupNewNameExists)
        {
            this.NamedFiles.File.CreateFileOnDisk();
            if (fileNewNameExists)
            {
                this.NamedFiles.WithNewName.CreateFileOnDisk();
                Assert.AreEqual(false, this.Repository.CanRename(this.NamedFiles.File, "NewName"));
            }

            if (backupNewNameExists)
            {
                if (!this.IsBackingUp)
                {
                    return;
                }

                this.NamedFiles.BackupNewName.CreateFileOnDisk();
                this.NamedFiles.Backup.CreateFileOnDisk();
                this.NamedFiles.BackupNewName.CreateFileOnDisk();
                Assert.AreEqual(false, this.Repository.CanRename(this.NamedFiles.File, "NewName"));
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

        protected abstract IRepository CreateRepository();

        protected abstract void Save<T>(System.IO.FileInfo file, T item);

        protected abstract T Read<T>(System.IO.FileInfo file);
    }
}