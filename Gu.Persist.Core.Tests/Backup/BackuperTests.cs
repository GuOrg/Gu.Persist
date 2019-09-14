#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Backup
{
    using System;
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    [NonParallelizable]
    public class BackuperTests : BackupTests
    {
        private DummySerializable dummy;

        [SetUp]
        public override void SetUp()
        {
            this.dummy = new DummySerializable(1);
        }

        [Test]
        public void BackupWhenNotExists()
        {
            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, null, 1, 3));
            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.Backup);

            Assert.IsFalse(backuper.BeforeSave(this.File));

            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.Backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDelete()
        {
            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
            this.SoftDelete.Save(this.dummy);
            AssertFile.Exists(false, this.File);
            AssertFile.Exists(true, this.SoftDelete);

            Assert.IsTrue(backuper.CanRestore(this.File));
            Assert.IsTrue(backuper.TryRestore(this.File));

            Assert.AreEqual(this.dummy, this.File.Read<DummySerializable>());
            AssertFile.Exists(true, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(false, this.Backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDeleteAndBackup()
        {
            this.Backup.Save(this.dummy);
            this.dummy.Value++;
            this.SoftDelete.Save(this.dummy);

            AssertFile.Exists(false, this.File);
            AssertFile.Exists(true, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);

            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));

            Assert.IsTrue(backuper.CanRestore(this.File));
            Assert.IsTrue(backuper.TryRestore(this.File));

            Assert.AreEqual(this.dummy, this.File.Read<DummySerializable>());
            AssertFile.Exists(true, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);
        }

        [Test]
        public void TryRestoreWhenHasBackupAndOriginal()
        {
            this.File.WriteAllText("File");
            this.Backup.Save(this.dummy);

            AssertFile.Exists(true, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);

            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));

            var exception = Assert.Throws<InvalidOperationException>(() => backuper.TryRestore(this.File));
            StringAssert.IsMatch(@"Expected file .*BackuperTests\\Meh.cfg to not exist.", exception.Message);

            AssertFile.Exists(true, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);
        }

        [Test]
        public void TryRestoreWhenHasBackup()
        {
            this.Backup.Save(this.dummy);

            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);

            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
            Assert.IsTrue(backuper.CanRestore(this.File));
            Assert.IsTrue(backuper.TryRestore(this.File));

            Assert.AreEqual(this.dummy, this.File.Read<DummySerializable>());
            AssertFile.Exists(true, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(false, this.Backup);
        }

        [Test]
        public void TryRestoreWhenNoFiles()
        {
            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(false, this.Backup);

            var backuper = Backuper.Create(new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
            Assert.IsFalse(backuper.TryRestore(this.File));

            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(false, this.Backup);
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
    }
}
