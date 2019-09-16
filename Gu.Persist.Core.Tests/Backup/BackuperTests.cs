#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Backup
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    [NonParallelizable]
    public static class BackuperTests
    {
        private static readonly DirectoryInfo Directory = Directories.TempDirectory
                                                                     .CreateSubdirectory("Gu.Persist.Tests")
                                                                     .CreateSubdirectory(typeof(BackuperTests).FullName);

        [SetUp]
        public static void SetUp()
        {
            Directory.Refresh();
            Directory.CreateIfNotExists();
        }

        [TearDown]
        public static void TearDown()
        {
            Directory.DeleteIfExists(true);
        }

        [Test]
        public static void BeforeSaveWhenFileDoesNotNotExists()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var backuper = Backuper.Create(new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, null, 1, 3));
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, backup);

            Assert.AreEqual(false, backuper.BeforeSave(file));

            AssertFile.Exists(false, file);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public static void BeforeSaveWhenFileExists()
        {
            var file = CreateFile();
            file.CreateFileOnDisk("abc");
            var backup = CreateBackupFile();
            var backuper = Backuper.Create(new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, null, 1, 3));
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, backup);

            Assert.AreEqual(true, backuper.BeforeSave(file));

            AssertFile.Exists(true, file);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public static void TryRestoreWhenHasSoftDelete()
        {
            var dummy = new DummySerializable(1);
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            var backuper = Backuper.Create(new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
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
        public static void TryRestoreWhenHasSoftDeleteAndBackup()
        {
            var dummy = new DummySerializable(1);
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            backup.Save(dummy);
            dummy.Value++;
            softDelete.Save(dummy);

            AssertFile.Exists(false, file);
            AssertFile.Exists(true, softDelete);
            AssertFile.Exists(true, backup);

            var backuper = Backuper.Create(new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));

            Assert.IsTrue(backuper.CanRestore(file));
            Assert.IsTrue(backuper.TryRestore(file));

            Assert.AreEqual(dummy, file.Read<DummySerializable>());
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public static void TryRestoreWhenHasBackupAndOriginal()
        {
            var dummy = new DummySerializable(-1);
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            file.WriteAllText("File");
            backup.Save(dummy);

            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);

            var backuper = Backuper.Create(new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));

            var exception = Assert.Throws<InvalidOperationException>(() => backuper.TryRestore(file));
            StringAssert.IsMatch(@"Expected file .*BackuperTests\\TryRestoreWhenHasBackupAndOriginal.cfg to not exist.", exception.Message);

            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public static void TryRestoreWhenHasBackup()
        {
            var dummy = new DummySerializable(-1);
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            backup.Save(dummy);

            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);

            var backuper = Backuper.Create(new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
            Assert.IsTrue(backuper.CanRestore(file));
            Assert.IsTrue(backuper.TryRestore(file));

            Assert.AreEqual(dummy, file.Read<DummySerializable>());
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public static void TryRestoreWhenNoFiles()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);

            var backuper = Backuper.Create(new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 3));
            Assert.IsFalse(backuper.TryRestore(file));

            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public static void PurgeAll()
        {
            Assert.Inconclusive("do we want this?");
        }

        [Test]
        public static void AfterSaveWhenNoFiles()
        {
            var file = CreateFile();
            var backuper = Backuper.Create(new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 2, 2));

            using (var lockedFile = Core.LockedFile.CreateIfExists(file, x => x.OpenRead()))
            {
                backuper.AfterSave(lockedFile);
            }
        }

        [Test]
        public static void AfterSavePurgeNumberOfFiles()
        {
            var settings = new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 3, int.MaxValue);
            var file = CreateFile();
            var backup = CreateBackupFile();
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
        public static void AfterSavePurgeOld()
        {
            var settings = new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, int.MaxValue, 2);
            var file = CreateFile();
            var backup = CreateBackupFile();
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
        public static void AfterSaveDeletesSoftDeletesWhenNoPurgeOfBackups()
        {
            var settings = new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, int.MaxValue, int.MaxValue);
            var file = CreateFile();
            var backup = CreateBackupFile();
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
        public static void Rename()
        {
            Assert.Inconclusive("Backups must be renamed when original file is renamed");
        }

        private static FileInfo CreateFile([CallerMemberName] string name = null) => Directory.CreateFileInfoInDirectory(name + ".cfg");

        private static FileInfo CreateBackupFile([CallerMemberName] string name = null) => Directory.CreateFileInfoInDirectory(name + ".bak");
    }
}
