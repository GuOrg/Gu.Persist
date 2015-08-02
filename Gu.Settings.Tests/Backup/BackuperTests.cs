
namespace Gu.Settings.Tests.Backup
{
    using System;

    using Gu.Settings.Backup;

    using NUnit.Framework;

    public class BackuperTests : BackupTests
    {
        private Backuper _backuper;
        private DummySerializable _dummy;

        [SetUp]
        public override void SetUp()
        {
            _backuper = (Backuper)Backuper.Create(Setting);
            _dummy = new DummySerializable(1);
        }

        [Test]
        public void BackupWhenNotExtsis()
        {
            Setting.NumberOfBackups = 1;
            Setting.TimeStampFormat = null;
            AssertFile.Exists(false, File);
            AssertFile.Exists(false, Backup);

            Assert.IsFalse(_backuper.TryBackup(File));

            AssertFile.Exists(false, File);
            AssertFile.Exists(false, Backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDelete()
        {
            SoftDelete.Save(_dummy);
            AssertFile.Exists(false, File);
            AssertFile.Exists(true, SoftDelete);

            Assert.IsTrue(_backuper.CanRestore(File));
            Assert.IsTrue(_backuper.TryRestore(File));

            Assert.AreEqual(_dummy, File.Read<DummySerializable>());
            AssertFile.Exists(true, File);
            AssertFile.Exists(false, SoftDelete);
            AssertFile.Exists(false, Backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDeleteAndBackup()
        {
            Backup.Save(_dummy);
            _dummy.Value++;
            SoftDelete.Save(_dummy);

            AssertFile.Exists(false, File);
            AssertFile.Exists(true, SoftDelete);
            AssertFile.Exists(true, Backup);

            Assert.IsTrue(_backuper.CanRestore(File));
            Assert.IsTrue(_backuper.TryRestore(File));

            Assert.AreEqual(_dummy, File.Read<DummySerializable>());
            AssertFile.Exists(true, File);
            AssertFile.Exists(false, SoftDelete);
            AssertFile.Exists(true, Backup);
        }

        [Test]
        public void TryRestoreWhenHasBackupAndOriginal()
        {
            File.WriteAllText("File");
            Backup.Save(_dummy);

            AssertFile.Exists(true, File);
            AssertFile.Exists(false, SoftDelete);
            AssertFile.Exists(true, Backup);

            Assert.Throws<InvalidOperationException>(() => _backuper.TryRestore(File));

            AssertFile.Exists(true, File);
            AssertFile.Exists(false, SoftDelete);
            AssertFile.Exists(true, Backup);
        }

        [Test]
        public void TryRestoreWhenHasBackup()
        {
            Backup.Save(_dummy);

            AssertFile.Exists(false, File);
            AssertFile.Exists(false, SoftDelete);
            AssertFile.Exists(true, Backup);

            Assert.IsTrue(_backuper.CanRestore(File));
            Assert.IsTrue(_backuper.TryRestore(File));

            Assert.AreEqual(_dummy, File.Read<DummySerializable>());
            AssertFile.Exists(true, File);
            AssertFile.Exists(false, SoftDelete);
            AssertFile.Exists(false, Backup);
        }

        [Test]
        public void TryRestoreWhenNoFiles()
        {
            AssertFile.Exists(false, File);
            AssertFile.Exists(false, SoftDelete);
            AssertFile.Exists(false, Backup);

            Assert.IsFalse(_backuper.TryRestore(File));

            AssertFile.Exists(false, File);
            AssertFile.Exists(false, SoftDelete);
            AssertFile.Exists(false, Backup);
        }

        [Test]
        public void PurgeAll()
        {
            Assert.Inconclusive("do we want this?");
        }

        [Test]
        public void PurgeWhenNoFiles()
        {
            Setting.NumberOfBackups = 2;
            Setting.MaxAgeInDays = 2;
            _backuper.PurgeBackups(File);
            AssertFile.Exists(false, BackupOneMinuteOld);
            AssertFile.Exists(false, BackupOneHourOld);
            AssertFile.Exists(false, BackupOneDayOld);
            AssertFile.Exists(false, BackupOneMonthOld);
            AssertFile.Exists(false, BackupOneYearOld);
            //AssertFile.Exists(false, _softDelete);
        }

        [Test]
        public void PurgeNumberOfFiles()
        {
            SoftDelete.VoidCreate();
            foreach (var backup in TimestampedBackups)
            {
                backup.VoidCreate();
            }
            Setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            Setting.NumberOfBackups = 3;
            Setting.MaxAgeInDays = int.MaxValue;
            _backuper.PurgeBackups(File);
            AssertFile.Exists(true, BackupOneMinuteOld);
            AssertFile.Exists(true, BackupOneHourOld);
            AssertFile.Exists(true, BackupOneDayOld);
            AssertFile.Exists(false, BackupOneMonthOld);
            AssertFile.Exists(false, BackupOneYearOld);
            AssertFile.Exists(false, SoftDelete);
        }

        [Test]
        public void PurgeOld()
        {
            SoftDelete.VoidCreate();
            foreach (var backup in TimestampedBackups)
            {
                backup.VoidCreate();
            }
            Setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            Setting.NumberOfBackups = int.MaxValue;
            Setting.MaxAgeInDays = 2;
            _backuper.PurgeBackups(File);
            AssertFile.Exists(true, BackupOneMinuteOld);
            AssertFile.Exists(true, BackupOneHourOld);
            AssertFile.Exists(true, BackupOneDayOld);
            AssertFile.Exists(false, BackupOneMonthOld);
            AssertFile.Exists(false, BackupOneYearOld);
            AssertFile.Exists(false, SoftDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletesNoTimestamp()
        {
            File.VoidCreate();
            SoftDelete.VoidCreate();
            Backup.VoidCreate();
            _backuper.PurgeBackups(File);
            AssertFile.Exists(true, File);
            AssertFile.Exists(true, Backup);
            AssertFile.Exists(false, SoftDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletes()
        {
            foreach (var backup in TimestampedBackups)
            {
                backup.VoidCreate();
            }
            SoftDelete.VoidCreate();
            File.VoidCreate();
            Backup.VoidCreate();
            Setting.NumberOfBackups = int.MaxValue;
            Setting.MaxAgeInDays = int.MaxValue;
            _backuper.PurgeBackups(File);
            AssertFile.Exists(true, File);
            AssertFile.Exists(true, Backup);
            AssertFile.Exists(false, SoftDelete);
        }

        [Test]
        public void Rename()
        {
            Assert.Inconclusive("Backups must be renamed when original file is renamed");
        }
    }
}
