#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Backup
{
    using System;
    using System.IO;

    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    public class NullBackuperTests
    {
        protected readonly DirectoryInfo Directory;
        protected FileInfo File;
        protected FileInfo Backup;
        protected FileInfo BackupOneMinuteOld;
        protected FileInfo BackupOneHourOld;
        protected FileInfo BackupOneDayOld;
        protected FileInfo BackupOneMonthOld;
        protected FileInfo BackupOneYearOld;
        protected FileInfo[] TimestampedBackups;
        protected FileInfo SoftDelete;
        protected FileInfo OtherBackup;

        public NullBackuperTests()
        {
            this.Directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                        .CreateSubdirectory(this.GetType().FullName);
            _ = this.Directory.CreateIfNotExists();

            this.File = this.Directory.CreateFileInfoInDirectory("Meh.cfg");
            this.SoftDelete = this.File.WithAppendedExtension(FileHelper.SoftDeleteExtension);
            this.Backup = this.Directory.CreateFileInfoInDirectory("Meh.bak");
            var settings = new BackupSettings(this.Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 3, 3);
            this.BackupOneMinuteOld = this.Backup.WithTimeStamp(DateTime.Now.AddMinutes(-1), settings);
            this.BackupOneHourOld = this.Backup.WithTimeStamp(DateTime.Now.AddHours(-1), settings);
            this.BackupOneDayOld = this.Backup.WithTimeStamp(DateTime.Now.AddDays(-1), settings);
            this.BackupOneMonthOld = this.Backup.WithTimeStamp(DateTime.Now.AddMonths(-1), settings);
            this.BackupOneYearOld = this.Backup.WithTimeStamp(DateTime.Now.AddYears(-1), settings);

            this.OtherBackup = this.Directory.CreateFileInfoInDirectory("Other.bak").WithTimeStamp(DateTime.Now.AddHours(1), settings);

            this.TimestampedBackups = new[]
                                      {
                                          this.BackupOneMinuteOld,
                                          this.BackupOneHourOld,
                                          this.BackupOneDayOld,
                                          this.BackupOneMonthOld,
                                          this.BackupOneYearOld,
                                      };
        }

        [SetUp]
        public virtual void SetUp()
        {
            this.File.Delete();
            this.Backup.Delete();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.Delete();
            }

            this.OtherBackup.Delete();
            this.OtherBackup.CreateFileOnDisk();
            this.SoftDelete.Delete();
        }

        [TearDown]
        public void TearDown()
        {
            this.File.Delete();
            this.Backup.Delete();

            foreach (var backup in this.TimestampedBackups)
            {
                backup.Delete();
            }

            this.OtherBackup.Delete();
            this.SoftDelete.Delete();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.Directory.Delete(recursive: true);
        }

        [Test]
        public void BackupWhenNotExists()
        {
            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.Backup);

            Assert.AreEqual(false, NullBackuper.Default.BeforeSave(this.File));

            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.Backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDelete()
        {
            var dummy = new DummySerializable(1);
            this.SoftDelete.Save(dummy);
            AssertFile.Exists(false, this.File);
            AssertFile.Exists(true, this.SoftDelete);

            Assert.IsTrue(NullBackuper.Default.CanRestore(this.File));
            Assert.IsTrue(NullBackuper.Default.TryRestore(this.File));

            Assert.AreEqual(dummy, this.File.Read<DummySerializable>());
            AssertFile.Exists(true, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(false, this.Backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDeleteAndBackup()
        {
            var dummy = new DummySerializable(1);
            this.Backup.Save(dummy);
            dummy.Value++;
            this.SoftDelete.Save(dummy);

            AssertFile.Exists(false, this.File);
            AssertFile.Exists(true, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);

            Assert.IsTrue(NullBackuper.Default.CanRestore(this.File));
            Assert.IsTrue(NullBackuper.Default.TryRestore(this.File));

            Assert.AreEqual(dummy, this.File.Read<DummySerializable>());
            AssertFile.Exists(true, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);
        }

        [Test]
        public void TryRestoreWhenHasBackupAndOriginal()
        {
            this.File.WriteAllText("File");
            this.SoftDelete.CreateFileOnDisk();
            this.Backup.CreateFileOnDisk();

            AssertFile.Exists(true, this.File);
            AssertFile.Exists(true, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);

            var exception = Assert.Throws<InvalidOperationException>(() => NullBackuper.Default.TryRestore(this.File));
            StringAssert.IsMatch(@"Expected file .+NullBackuperTests\\Meh.cfg to not exist.", exception.Message);

            AssertFile.Exists(true, this.File);
            AssertFile.Exists(true, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);
        }

        [Test]
        public void TryRestoreWhenHasBackup()
        {
            var dummy = new DummySerializable(1);
            this.Backup.Save(dummy);

            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);

            Assert.IsFalse(NullBackuper.Default.CanRestore(this.File));
            Assert.IsFalse(NullBackuper.Default.TryRestore(this.File));

            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(true, this.Backup);
        }

        [Test]
        public void TryRestoreWhenNoFiles()
        {
            AssertFile.Exists(false, this.File);
            AssertFile.Exists(false, this.SoftDelete);
            AssertFile.Exists(false, this.Backup);

            Assert.IsFalse(NullBackuper.Default.TryRestore(this.File));

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
            using (var lockedFile = this.LockedFile())
            {
                NullBackuper.Default.AfterSave(lockedFile);
            }

            AssertFile.Exists(false, this.BackupOneMinuteOld);
            AssertFile.Exists(false, this.BackupOneHourOld);
            AssertFile.Exists(false, this.BackupOneDayOld);
            AssertFile.Exists(false, this.BackupOneMonthOld);
            AssertFile.Exists(false, this.BackupOneYearOld);
            AssertFile.Exists(false, this.SoftDelete);
        }

        [Test]
        public void PurgeWhenHasSoftDelete()
        {
            this.File.CreateFileOnDisk();
            this.SoftDelete.CreateFileOnDisk();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.CreateFileOnDisk();
            }

            using (var lockedFile = this.LockedFile())
            {
                NullBackuper.Default.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, this.File);
            AssertFile.Exists(true, this.BackupOneMinuteOld);
            AssertFile.Exists(true, this.BackupOneHourOld);
            AssertFile.Exists(true, this.BackupOneDayOld);
            AssertFile.Exists(true, this.BackupOneMonthOld);
            AssertFile.Exists(true, this.BackupOneYearOld);
            AssertFile.Exists(false, this.SoftDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletesNoTimestamp()
        {
            this.File.CreateFileOnDisk();
            this.SoftDelete.CreateFileOnDisk();
            this.Backup.CreateFileOnDisk();
            using (var lockedFile = this.LockedFile())
            {
                NullBackuper.Default.AfterSave(lockedFile);
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
            using (var lockedFile = this.LockedFile())
            {
                NullBackuper.Default.AfterSave(lockedFile);
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
