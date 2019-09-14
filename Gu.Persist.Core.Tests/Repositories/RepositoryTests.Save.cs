#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public abstract partial class RepositoryTests
    {
        [Test]
        public void SaveFileInfo()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            AssertFile.Exists(false, file);
            this.Repository.Save(file, dummy);
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

        [Test]
        public void SaveFullName()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            AssertFile.Exists(false, file);
            this.Repository.Save(file.FullName, dummy);
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

        [Test]
        public void SaveName()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            AssertFile.Exists(false, file);
            this.Repository.Save(file.Name, dummy);
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

        [Test]
        public void SaveGeneric()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);
            var backupFile = repository.GetGenericTestBackupFileInfo(dummy);
            AssertFile.Exists(false, file);
            this.Repository.Save(dummy);
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

        [Test]
        public void SaveFileInfoTwice()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            AssertFile.Exists(false, file);
            this.Repository.Save(file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

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
        public void SaveFullNameTwice()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            AssertFile.Exists(false, file);
            this.Repository.Save(file.FullName, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

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
        public void SaveNameTwice()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            AssertFile.Exists(false, file);
            this.Repository.Save(file.Name, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

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
        public void SaveGenericTwice()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);
            var backupFile = repository.GetGenericTestBackupFileInfo(dummy);
            AssertFile.Exists(false, file);
            this.Repository.Save(dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

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
        public void SaveFileInfoThrice()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            AssertFile.Exists(false, file);
            this.Repository.Save(file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            this.Repository.Save(file, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

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
        public void SaveFullNameThrice()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            AssertFile.Exists(false, file);
            this.Repository.Save(file.FullName, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            this.Repository.Save(file.FullName, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

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
        public void SaveNameThrice()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            AssertFile.Exists(false, file);
            this.Repository.Save(file.Name, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            this.Repository.Save(file.Name, dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

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
        public void SaveGenericThrice()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);
            var backupFile = repository.GetGenericTestBackupFileInfo(dummy);
            AssertFile.Exists(false, file);
            this.Repository.Save(dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            this.Repository.Save(dummy);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, file.GetSoftDeleteFileFor());
            AssertFile.Exists(false, file.TempFile(repository.Settings));
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(true, backupFile);
            }

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
        public void SaveThenReadFileInfo()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            AssertFile.Exists(false, file);
            repository.Save(file, dummy);
            AssertFile.Exists(true, file);

            var read = repository.Read<DummySerializable>(file);
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
            var file = repository.GetTestFileInfo();
            AssertFile.Exists(false, file);
            repository.Save(file.FullName, dummy);
            AssertFile.Exists(true, file);

            var read = repository.Read<DummySerializable>(file.FullName);
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
            var file = repository.GetTestFileInfo();
            AssertFile.Exists(false, file);
            repository.Save(file.Name, dummy);
            AssertFile.Exists(true, file);

            var read = repository.Read<DummySerializable>(file.Name);
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
            var file = repository.GetGenericTestFileInfo(dummy);
            AssertFile.Exists(false, file);
            repository.Save(dummy);
            AssertFile.Exists(true, file);

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
            var file = repository.GetTestFileInfo();
            AssertFile.Exists(false, file);
            await repository.SaveAsync(file, dummy).ConfigureAwait(false);
            AssertFile.Exists(true, file);

            var read = await repository.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
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
            var file = repository.GetTestFileInfo();
            AssertFile.Exists(false, file);
            await repository.SaveAsync(file.FullName, dummy).ConfigureAwait(false);
            AssertFile.Exists(true, file);

            var read = await repository.ReadAsync<DummySerializable>(file.FullName).ConfigureAwait(false);
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
            var file = repository.GetTestFileInfo();
            AssertFile.Exists(false, file);
            await repository.SaveAsync(file.Name, dummy).ConfigureAwait(false);
            AssertFile.Exists(true, file);

            var read = await repository.ReadAsync<DummySerializable>(file.Name).ConfigureAwait(false);
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
            var file = repository.GetGenericTestFileInfo(dummy);
            AssertFile.Exists(false, file);
            await repository.SaveAsync(dummy).ConfigureAwait(false);
            AssertFile.Exists(true, file);

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
        public void SaveThenSaveNullFileInfo()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            this.Repository.Save(file, dummy);
            AssertFile.Exists(true, file);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            if ((repository.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                repository.Save<DummySerializable>(file, null);
                AssertFile.Exists(false, file);
                if (repository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(true, backupFile);
                }
            }
            else
            {
                _ = Assert.Throws<ArgumentNullException>(() => repository.Save<DummySerializable>(file, null));
            }
        }

        [Test]
        public void SaveThenSaveNullFullName()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            this.Repository.Save(file.FullName, dummy);
            AssertFile.Exists(true, file);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            if ((repository.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                repository.Save<DummySerializable>(file.FullName, null);
                AssertFile.Exists(false, file);
                if (repository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(true, backupFile);
                }
            }
            else
            {
                _ = Assert.Throws<ArgumentNullException>(() => repository.Save<DummySerializable>(file.FullName, null));
            }
        }

        [Test]
        public void SaveThenSaveNullName()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            this.Repository.Save(file.Name, dummy);
            AssertFile.Exists(true, file);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            if ((repository.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                repository.Save<DummySerializable>(file.Name, null);
                AssertFile.Exists(false, file);
                if (repository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(true, backupFile);
                }
            }
            else
            {
                _ = Assert.Throws<ArgumentNullException>(() => repository.Save<DummySerializable>(file.Name, null));
            }
        }

        [Test]
        public void SaveThenSaveNullGeneric()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);
            var backupFile = repository.GetGenericTestBackupFileInfo(dummy);
            this.Repository.Save(dummy);
            AssertFile.Exists(true, file);
            if (repository.Settings.BackupSettings != null)
            {
                AssertFile.Exists(false, backupFile);
            }

            if ((repository.Settings as IDataRepositorySettings)?.SaveNullDeletesFile == true)
            {
                repository.Save<DummySerializable>(null);
                AssertFile.Exists(false, file);
                if (repository.Settings.BackupSettings != null)
                {
                    AssertFile.Exists(true, backupFile);
                }
            }
            else
            {
                _ = Assert.Throws<ArgumentNullException>(() => repository.Save<DummySerializable>(null));
            }
        }

        [Test]
        public void SaveFileInfoWhenTempFileExists()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            file.TempFile(repository.Settings).CreateFileOnDisk();
            this.Repository.Save(file, dummy);
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

        [Test]
        public void SaveFullNameWhenTempFileExists()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            file.TempFile(repository.Settings).CreateFileOnDisk();
            this.Repository.Save(file.FullName, dummy);
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

        [Test]
        public void SaveNameWhenTempFileExists()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var backupFile = repository.GetTestBackupFileInfo();
            file.TempFile(repository.Settings).CreateFileOnDisk();
            this.Repository.Save(file.Name, dummy);
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

        [Test]
        public void SaveGenericWhenTempFileExists()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);
            var backupFile = repository.GetGenericTestBackupFileInfo(dummy);
            file.TempFile(repository.Settings).CreateFileOnDisk();
            this.Repository.Save(dummy);
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
    }
}