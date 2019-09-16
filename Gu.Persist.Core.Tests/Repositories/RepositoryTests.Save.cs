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
        }

        [TestCaseSource(nameof(TestCases))]
        public void SaveWhenFileAndSoftDeleteExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            file.CreateFileOnDisk();
            file.TempFile(repository.Settings).CreateFileOnDisk();
            file.GetSoftDeleteFileFor().CreateFileOnDisk();
            testCase.Save(repository, file, dummy);
            if (repository.Settings.BackupSettings != null)
            {
                var backupFile = testCase.BackupFile<DummySerializable>(repository);
                AssertFile.Exists(true, backupFile);
            }

            var read = this.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
        }

        [TestCaseSource(nameof(TestCases))]
        public void SaveLongListThenShortList(TestCase testCase)
        {
            var list = new List<DummySerializable>
            {
                new DummySerializable(1),
                new DummySerializable(2),
            };
            var repository = this.CreateRepository();
            var file = testCase.File<List<DummySerializable>>(repository);
            testCase.Save(repository, file, list);
            var read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);

            list.RemoveAt(0);
            testCase.Save(repository, file, list);
            read = this.Read<List<DummySerializable>>(file);
            CollectionAssert.AreEqual(list, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void SaveManagesSingleton(TestCase testCase)
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

            dummy.Value++;
            testCase.Save(repository, file, dummy);
            read = testCase.Read<DummySerializable>(repository, file);
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
        public void IsDirtyBeforeFirstSave(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            if (repository.Settings.IsTrackingDirty)
            {
                Assert.AreEqual(true, testCase.IsDirty(repository, file, dummy));

                testCase.Save(repository, file, dummy);
                Assert.AreEqual(false, testCase.IsDirty(repository, file, dummy));

                dummy.Value++;
                Assert.AreEqual(true, testCase.IsDirty(repository, file, dummy));

                testCase.Save(repository, file, dummy);
                Assert.AreEqual(false, testCase.IsDirty(repository, file, dummy));
            }
            else
            {
                var exception = Assert.Throws<InvalidOperationException>(() => repository.IsDirty(dummy));
                Assert.AreEqual("This repository is not tracking dirty.", exception.Message);
            }
        }
    }
}