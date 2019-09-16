#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Backup
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    public class BackuperTests : BackupTests
    {
        private readonly DirectoryInfo directory;

        public BackuperTests()
        {
            this.directory = Directories.TempDirectory
                                        .CreateSubdirectory("Gu.Persist.Tests")
                                        .CreateSubdirectory(this.GetType().FullName);
        }

        [SetUp]
        public override void SetUp()
        {
            this.directory.CreateIfNotExists();
            base.SetUp();
        }

        [TearDown]
        public new void TearDown()
        {
            //this.directory.Delete(true);
        }

        [Test]
        public void BackupWhenNotExists()
        {
            var file = this.CreateFile();
            var backup = this.CreateBackupFile();
            var backuper = Backuper.Create(new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, null, 1, 3));
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, backup);

            Assert.AreEqual(false, backuper.BeforeSave(file));

            AssertFile.Exists(false, file);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public void BackupWhenExists()
        {
            var file = this.CreateFile();
            file.CreateFileOnDisk("abc");
            var backup = this.CreateBackupFile();
            var backuper = Backuper.Create(new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, null, 1, 3));
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, backup);

            Assert.AreEqual(true, backuper.BeforeSave(file));

            AssertFile.Exists(true, file);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDelete()
        {
            var dummy = new DummySerializable(1);
            var file = this.CreateFile();
            var backup = this.CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            var backuper = Backuper.Create(new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
            softDelete.Save(dummy);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, softDelete);

            Assert.IsTrue(backuper.CanRestore(file));
            Assert.IsTrue(backuper.TryRestore(file));

            Assert.AreEqual(dummy, file.Read<DummySerializable>());
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDeleteAndBackup()
        {
            var dummy = new DummySerializable(1);
            var file = this.CreateFile();
            var backup = this.CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            backup.Save(dummy);
            dummy.Value++;
            softDelete.Save(dummy);

            AssertFile.Exists(false, file);
            AssertFile.Exists(true, softDelete);
            AssertFile.Exists(true, backup);

            var backuper = Backuper.Create(new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));

            Assert.IsTrue(backuper.CanRestore(file));
            Assert.IsTrue(backuper.TryRestore(file));

            Assert.AreEqual(dummy, file.Read<DummySerializable>());
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void TryRestoreWhenHasBackupAndOriginal()
        {
            var dummy = new DummySerializable(-1);
            var file = this.CreateFile();
            var backup = this.CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            file.WriteAllText("File");
            backup.Save(dummy);

            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);

            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));

            var exception = Assert.Throws<InvalidOperationException>(() => backuper.TryRestore(file));
            StringAssert.IsMatch(@"Expected file .*BackuperTests\\TryRestoreWhenHasBackupAndOriginal.cfg to not exist.", exception.Message);

            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void TryRestoreWhenHasBackup()
        {
            var dummy = new DummySerializable(-1);
            var file = this.CreateFile();
            var backup = this.CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            backup.Save(dummy);

            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);

            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
            Assert.IsTrue(backuper.CanRestore(file));
            Assert.IsTrue(backuper.TryRestore(file));

            Assert.AreEqual(dummy, file.Read<DummySerializable>());
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public void TryRestoreWhenNoFiles()
        {
            var file = this.CreateFile();
            var backup = this.CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);

            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
            Assert.IsFalse(backuper.TryRestore(file));

            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public void PurgeAll()
        {
            Assert.Inconclusive("do we want this?");
        }

        [Test]
        public void PurgeWhenNoFiles()
        {
            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 2));

            using (var lockedFile = this.LockedFile())
            {
                backuper.AfterSave(lockedFile);
            }

            AssertFile.Exists(false, this.BackupOneMinuteOld);
            AssertFile.Exists(false, this.BackupOneHourOld);
            AssertFile.Exists(false, this.BackupOneDayOld);
            AssertFile.Exists(false, this.BackupOneMonthOld);
            AssertFile.Exists(false, this.BackupOneYearOld);
            ////AssertFile.Exists(false, _softDelete);
        }

        [Test]
        public void PurgeNumberOfFiles()
        {
            this.SoftDelete.CreateFileOnDisk();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.CreateFileOnDisk();
            }

            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 3, int.MaxValue));
            using (var lockedFile = this.LockedFile())
            {
                backuper.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, this.BackupOneMinuteOld);
            AssertFile.Exists(true, this.BackupOneHourOld);
            AssertFile.Exists(true, this.BackupOneDayOld);
            AssertFile.Exists(false, this.BackupOneMonthOld);
            AssertFile.Exists(false, this.BackupOneYearOld);
            AssertFile.Exists(false, this.SoftDelete);
        }

        [Test]
        public void PurgeOld()
        {
            this.SoftDelete.CreateFileOnDisk();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.CreateFileOnDisk();
            }

            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, int.MaxValue, 2));
            using (var lockedFile = this.LockedFile())
            {
                backuper.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, this.BackupOneMinuteOld);
            AssertFile.Exists(true, this.BackupOneHourOld);
            AssertFile.Exists(true, this.BackupOneDayOld);
            AssertFile.Exists(false, this.BackupOneMonthOld);
            AssertFile.Exists(false, this.BackupOneYearOld);
            AssertFile.Exists(false, this.SoftDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletesNoTimestamp()
        {
            this.File.CreateFileOnDisk();
            this.SoftDelete.CreateFileOnDisk();
            this.Backup.CreateFileOnDisk();
            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, int.MaxValue, int.MaxValue));
            using (var lockedFile = this.LockedFile())
            {
                backuper.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, this.File);
            AssertFile.Exists(true, this.Backup);
            AssertFile.Exists(false, this.SoftDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletes()
        {
            foreach (var backup in this.TimestampedBackups)
            {
                backup.CreateFileOnDisk();
            }

            this.SoftDelete.CreateFileOnDisk();
            this.File.CreateFileOnDisk();
            this.Backup.CreateFileOnDisk();
            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, int.MaxValue, int.MaxValue));
            using (var lockedFile = this.LockedFile())
            {
                backuper.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, this.File);
            AssertFile.Exists(true, this.Backup);
            AssertFile.Exists(false, this.SoftDelete);
        }

        [Test]
        public void Rename()
        {
            Assert.Inconclusive("Backups must be renamed when original file is renamed");
        }

        private LockedFile LockedFile() => Core.LockedFile.CreateIfExists(this.File, x => x.Open(FileMode.Open, FileAccess.Read, FileShare.Delete));

        private FileInfo CreateFile([CallerMemberName] string name = null) => this.directory.CreateFileInfoInDirectory(name + ".cfg");

        private FileInfo CreateBackupFile([CallerMemberName] string name = null) => this.directory.CreateFileInfoInDirectory(name + ".bak");
    }
}
