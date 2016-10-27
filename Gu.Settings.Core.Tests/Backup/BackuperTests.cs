
namespace Gu.Settings.Core.Tests.Backup
{
    using System;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Backup;

    using NUnit.Framework;

    public class BackuperTests : BackupTests
    {
        private Backuper backuper;
        private DummySerializable dummy;

        [SetUp]
        public override void SetUp()
        {
            this.backuper = (Backuper)Backuper.Create(this.Setting);
            this.dummy = new DummySerializable(1);
        }

        [Test]
        public void BackupWhenNotExtsis()
        {
            this.Setting.NumberOfBackups = 1;
            this.Setting.TimeStampFormat = null;
            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.Backup);

            Assert.IsFalse(this.backuper.BeforeSave(this.File));

            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.Backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDelete()
        {
            this.SoftDelete.Save(this.dummy);
            AssertFile.Exists(false, this.File);
            AssertFile.Exists(true, this.SoftDelete);

            Assert.IsTrue(this.backuper.CanRestore(this.File));
            Assert.IsTrue(this.backuper.TryRestore(this.File));

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

            Assert.IsTrue(this.backuper.CanRestore(this.File));
            Assert.IsTrue(this.backuper.TryRestore(this.File));

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

            Assert.Throws<InvalidOperationException>(() => this.backuper.TryRestore(this.File));

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

            Assert.IsTrue(this.backuper.CanRestore(this.File));
            Assert.IsTrue(this.backuper.TryRestore(this.File));

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

            Assert.IsFalse(this.backuper.TryRestore(this.File));

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
            this.Setting.NumberOfBackups = 2;
            this.Setting.MaxAgeInDays = 2;
            this.backuper.AfterSuccessfulSave(this.File);
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
            this.SoftDelete.VoidCreate();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.VoidCreate();
            }

            this.Setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            this.Setting.NumberOfBackups = 3;
            this.Setting.MaxAgeInDays = int.MaxValue;
            this.backuper.AfterSuccessfulSave(this.File);
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
            this.SoftDelete.VoidCreate();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.VoidCreate();
            }

            this.Setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            this.Setting.NumberOfBackups = int.MaxValue;
            this.Setting.MaxAgeInDays = 2;
            this.backuper.AfterSuccessfulSave(this.File);
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
            this.File.VoidCreate();
            this.SoftDelete.VoidCreate();
            this.Backup.VoidCreate();
            this.backuper.AfterSuccessfulSave(this.File);
            AssertFile.Exists(true, this.File);
            AssertFile.Exists(true, this.Backup);
            AssertFile.Exists(false, this.SoftDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletes()
        {
            foreach (var backup in this.TimestampedBackups)
            {
                backup.VoidCreate();
            }

            this.SoftDelete.VoidCreate();
            this.File.VoidCreate();
            this.Backup.VoidCreate();
            this.Setting.NumberOfBackups = int.MaxValue;
            this.Setting.MaxAgeInDays = int.MaxValue;
            this.backuper.AfterSuccessfulSave(this.File);
            AssertFile.Exists(true, this.File);
            AssertFile.Exists(true, this.Backup);
            AssertFile.Exists(false, this.SoftDelete);
        }

        [Test]
        public void Rename()
        {
            Assert.Inconclusive("Backups must be renamed when original file is renamed");
        }
    }
}
