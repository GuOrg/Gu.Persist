#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    public abstract partial class RepositoryTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void Save(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var backupFile = testCase.BackupFile<DummySerializable>(repository);
            AssertFile.Exists(false, file);
            testCase.Save(repository, file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            var read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void SaveTwice(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var backupFile = testCase.BackupFile<DummySerializable>(repository);
            AssertFile.Exists(false, file);
            testCase.Save(repository, file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            var read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);

            testCase.Save(repository, file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void SaveThrice(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var backupFile = testCase.BackupFile<DummySerializable>(repository);
            AssertFile.Exists(false, file);
            testCase.Save(repository, file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            var read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);

            testCase.Save(repository, file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);

            testCase.Save(repository, file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void SaveThenRead(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            testCase.Save(repository, file, dummy);
            var read = testCase.Read<DummySerializable>(repository, file);
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

        [TestCaseSource(nameof(TestCases))]
        public void SaveThenSaveNull(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            testCase.Save(repository, file, dummy);
            if (repository.Settings is IDataRepositorySettings dataRepositorySettings &&
                dataRepositorySettings.SaveNullDeletesFile == true)
            {
                testCase.Save<DummySerializable>(repository, file, null);
                AssertFile.Exists(false, file);
                if (repository.Settings.BackupSettings != null)
                {
                    var backupFile = testCase.BackupFile<DummySerializable>(repository);
                    AssertFile.Exists(true, backupFile);
                }
            }
            else
            {
                _ = Assert.Throws<ArgumentNullException>(() => testCase.Save<DummySerializable>(repository, file, null));
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void SaveWhenTempFileExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            file.TempFile(repository.Settings).CreateFileOnDisk();
            testCase.Save(repository, file, dummy);
            if (repository.Settings.BackupSettings != null)
            {
                var backupFile = testCase.BackupFile<DummySerializable>(repository);
                AssertFile.Exists(false, backupFile);
            }

            var read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void SaveFileInfoWhenFileAndSoftDeleteExists()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            file.CreateFileOnDisk();
            file.GetSoftDeleteFileFor().CreateFileOnDisk();
            this.Repository.Save(file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            var read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void SaveFullNameWhenFileAndSoftDeleteExists()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            file.CreateFileOnDisk();
            file.GetSoftDeleteFileFor().CreateFileOnDisk();
            this.Repository.Save(file.FullName, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            var read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void SaveNameWhenFileAndSoftDeleteExists()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            file.CreateFileOnDisk();
            file.GetSoftDeleteFileFor().CreateFileOnDisk();
            this.Repository.Save(file.Name, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            var read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void SaveGenericWhenFileAndSoftDeleteExists()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);
            var backupFile = repository.GetGenericTestBackupFileInfo(dummy);
            file.CreateFileOnDisk();
            file.GetSoftDeleteFileFor().CreateFileOnDisk();
            this.Repository.Save(dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            var read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void SaveFileInfoLongListThenShortList()
        {
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            var list = new List<DummySerializable>
            {
                new DummySerializable(1),
                new DummySerializable(2),
            };

            repository.Save(file, list);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            var read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(list, read);

            list.RemoveAt(1);
            repository.Save(file, list);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(list, read);
        }

        [Test]
        public void SaveFullNameLongListThenShortList()
        {
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();

            var list = new List<DummySerializable>
            {
                new DummySerializable(1),
                new DummySerializable(2),
            };

            repository.Save(file.FullName, list);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            var read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(list, read);

            list.RemoveAt(1);
            repository.Save(file.FullName, list);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(list, read);
        }

        [Test]
        public void SaveNameLongListThenShortList()
        {
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();

            var list = new List<DummySerializable>
            {
                new DummySerializable(1),
                new DummySerializable(2),
            };

            repository.Save(file.Name, list);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            var read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(list, read);

            list.RemoveAt(1);
            repository.Save(file.Name, list);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(list, read);
        }

        [Test]
        public void SaveGenericLongListThenShortList()
        {
            var repository = this.CreateRepository();
            var list = new List<DummySerializable>
            {
                new DummySerializable(1),
                new DummySerializable(2),
            };
            var file = repository.GetGenericTestFileInfo(list);
            var backupFile = repository.GetGenericTestBackupFileInfo(list);

            repository.Save(list);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            var read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(list, read);

            list.RemoveAt(1);
            repository.Save(list);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

            read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);
            Assert.AreNotSame(list, read);
        }
    }
}