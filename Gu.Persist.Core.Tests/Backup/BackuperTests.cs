#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Backup
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    public class BackuperTests
    {
        private readonly DirectoryInfo directory;

        public BackuperTests()
        {
            this.directory = Directories.TempDirectory
                                        .CreateSubdirectory("Gu.Persist.Tests")
                                        .CreateSubdirectory(this.GetType().FullName);
        }

        [SetUp]
        public void SetUp()
        {
            this.directory.Refresh();
            this.directory.CreateIfNotExists();
        }

        [TearDown]
        public void TearDown()
        {
            this.directory.DeleteIfExists(true);
        }

        [Test]
        public void BeforeSaveWhenFileDoesNotNotExists()
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
        public void BeforeSaveWhenFileExists()
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

            var backuper = Backuper.Create(new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));

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

            var backuper = Backuper.Create(new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
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

            var backuper = Backuper.Create(new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
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
        public void AfterSaveWhenNoFiles()
        {
            var file = this.CreateFile();
            var backuper = Backuper.Create(new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 2));

            using (var lockedFile = Core.LockedFile.CreateIfExists(file, x => x.OpenRead()))
            {
                backuper.AfterSave(lockedFile);
            }
        }

        [Test]
        public void AfterSavePurgeNumberOfFiles()
        {
            var settings = new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 3, int.MaxValue);
            var file = this.CreateFile();
            var backup = this.CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            softDelete.CreateFileOnDisk();
            var backupOneMinuteOld = backup.WithTimeStamp(DateTime.Now.AddMinutes(-1), settings);
            backupOneMinuteOld.CreateFileOnDisk();
            var backupOneHourOld = backup.WithTimeStamp(DateTime.Now.AddHours(-1), settings);
            backupOneHourOld.CreateFileOnDisk();
            var backupOneDayOld = backup.WithTimeStamp(DateTime.Now.AddDays(-1), settings);
            backupOneDayOld.CreateFileOnDisk();
            var backupOneMonthOld = backup.WithTimeStamp(DateTime.Now.AddMonths(-1), settings);
            backupOneMonthOld.CreateFileOnDisk();
            var backupOneYearOld = backup.WithTimeStamp(DateTime.Now.AddYears(-1), settings);
            backupOneYearOld.CreateFileOnDisk();

            var backuper = Backuper.Create(settings);
            using (var lockedFile = Core.LockedFile.CreateIfExists(file, x => x.OpenRead()))
            {
                backuper.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, backupOneMinuteOld);
            AssertFile.Exists(true, backupOneHourOld);
            AssertFile.Exists(true, backupOneDayOld);
            AssertFile.Exists(false, backupOneMonthOld);
            AssertFile.Exists(false, backupOneYearOld);
            AssertFile.Exists(false, softDelete);
        }

        [Test]
        public void AfterSavePurgeOld()
        {
            var settings = new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, int.MaxValue, 2);
            var file = this.CreateFile();
            var backup = this.CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            softDelete.CreateFileOnDisk();
            var backupOneMinuteOld = backup.WithTimeStamp(DateTime.Now.AddMinutes(-1), settings);
            backupOneMinuteOld.CreateFileOnDisk();
            var backupOneHourOld = backup.WithTimeStamp(DateTime.Now.AddHours(-1), settings);
            backupOneHourOld.CreateFileOnDisk();
            var backupOneDayOld = backup.WithTimeStamp(DateTime.Now.AddDays(-1), settings);
            backupOneDayOld.CreateFileOnDisk();
            var backupOneMonthOld = backup.WithTimeStamp(DateTime.Now.AddMonths(-1), settings);
            backupOneMonthOld.CreateFileOnDisk();
            var backupOneYearOld = backup.WithTimeStamp(DateTime.Now.AddYears(-1), settings);
            backupOneYearOld.CreateFileOnDisk();

            var backuper = Backuper.Create(settings);
            using (var lockedFile = Core.LockedFile.CreateIfExists(file, x => x.OpenRead()))
            {
                backuper.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, backupOneMinuteOld);
            AssertFile.Exists(true, backupOneHourOld);
            AssertFile.Exists(true, backupOneDayOld);
            AssertFile.Exists(false, backupOneMonthOld);
            AssertFile.Exists(false, backupOneYearOld);
            AssertFile.Exists(false, softDelete);
        }

        [Test]
        public void AfterSaveDeletesSoftDeletesWhenNoPurgeOfBackups()
        {
            var settings = new BackupSettings(this.directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, int.MaxValue, int.MaxValue);
            var file = this.CreateFile();
            var backup = this.CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            softDelete.CreateFileOnDisk();
            var backupOneMinuteOld = backup.WithTimeStamp(DateTime.Now.AddMinutes(-1), settings);
            backupOneMinuteOld.CreateFileOnDisk();
            var backupOneHourOld = backup.WithTimeStamp(DateTime.Now.AddHours(-1), settings);
            backupOneHourOld.CreateFileOnDisk();
            var backupOneDayOld = backup.WithTimeStamp(DateTime.Now.AddDays(-1), settings);
            backupOneDayOld.CreateFileOnDisk();
            var backupOneMonthOld = backup.WithTimeStamp(DateTime.Now.AddMonths(-1), settings);
            backupOneMonthOld.CreateFileOnDisk();
            var backupOneYearOld = backup.WithTimeStamp(DateTime.Now.AddYears(-1), settings);
            backupOneYearOld.CreateFileOnDisk();

            var backuper = Backuper.Create(settings);
            using (var lockedFile = Core.LockedFile.CreateIfExists(file, x => x.OpenRead()))
            {
                backuper.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, backupOneMinuteOld);
            AssertFile.Exists(true, backupOneHourOld);
            AssertFile.Exists(true, backupOneDayOld);
            AssertFile.Exists(true, backupOneMonthOld);
            AssertFile.Exists(true, backupOneYearOld);
            AssertFile.Exists(false, softDelete);
        }

        [Test]
        public void Rename()
        {
            Assert.Inconclusive("Backups must be renamed when original file is renamed");
        }

        private FileInfo CreateFile([CallerMemberName] string name = null) => this.directory.CreateFileInfoInDirectory(name + ".cfg");

        private FileInfo CreateBackupFile([CallerMemberName] string name = null) => this.directory.CreateFileInfoInDirectory(name + ".bak");
    }
}
