#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Backup
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    public class NullBackuperTests
    {
        private readonly DirectoryInfo directory;
        private FileInfo file;
        private FileInfo backup;
        private FileInfo backupOneMinuteOld;
        private FileInfo backupOneHourOld;
        private FileInfo backupOneDayOld;
        private FileInfo backupOneMonthOld;
        private FileInfo backupOneYearOld;
        private FileInfo[] timestampedBackups;
        private FileInfo softDelete;
        private FileInfo otherBackup;

        public NullBackuperTests()
        {
            directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                        .CreateSubdirectory(GetType().FullName);
            _ = directory.CreateIfNotExists();

            file = directory.CreateFileInfoInDirectory("Meh.cfg");
            softDelete = file.WithAppendedExtension(FileHelper.SoftDeleteExtension);
            backup = directory.CreateFileInfoInDirectory("Meh.bak");
            var settings = new BackupSettings(directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 3, 3);
            backupOneMinuteOld = backup.WithTimeStamp(DateTime.Now.AddMinutes(-1), settings);
            backupOneHourOld = backup.WithTimeStamp(DateTime.Now.AddHours(-1), settings);
            backupOneDayOld = backup.WithTimeStamp(DateTime.Now.AddDays(-1), settings);
            backupOneMonthOld = backup.WithTimeStamp(DateTime.Now.AddMonths(-1), settings);
            backupOneYearOld = backup.WithTimeStamp(DateTime.Now.AddYears(-1), settings);

            otherBackup = directory.CreateFileInfoInDirectory("Other.bak").WithTimeStamp(DateTime.Now.AddHours(1), settings);

            timestampedBackups = new[]
                                      {
                                          backupOneMinuteOld,
                                          backupOneHourOld,
                                          backupOneDayOld,
                                          backupOneMonthOld,
                                          backupOneYearOld,
                                      };
        }

        [SetUp]
        public virtual void SetUp()
        {
            file.Delete();
            backup.Delete();
            foreach (var backup in timestampedBackups)
            {
                backup.Delete();
            }

            otherBackup.Delete();
            otherBackup.CreateFileOnDisk();
            softDelete.Delete();
        }

        [TearDown]
        public void TearDown()
        {
            file.Delete();
            backup.Delete();

            foreach (var backup in timestampedBackups)
            {
                backup.Delete();
            }

            otherBackup.Delete();
            softDelete.Delete();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            directory.Delete(recursive: true);
        }

        [Test]
        public void BackupWhenNotExists()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, backup);

            Assert.AreEqual(false, NullBackuper.Default.BeforeSave(file));

            AssertFile.Exists(false, file);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDelete()
        {
            var dummy = new DummySerializable(1);
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            softDelete.Save(dummy);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, softDelete);

            Assert.IsTrue(NullBackuper.Default.CanRestore(file));
            Assert.IsTrue(NullBackuper.Default.TryRestore(file));

            Assert.AreEqual(dummy, file.Read<DummySerializable>());
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public void TryRestoreWhenHasSoftDeleteAndBackup()
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

            Assert.IsTrue(NullBackuper.Default.CanRestore(file));
            Assert.IsTrue(NullBackuper.Default.TryRestore(file));

            Assert.AreEqual(dummy, file.Read<DummySerializable>());
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void TryRestoreWhenHasBackupAndOriginal()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            file.WriteAllText("File");
            softDelete.CreateFileOnDisk();
            backup.CreateFileOnDisk();

            AssertFile.Exists(true, file);
            AssertFile.Exists(true, softDelete);
            AssertFile.Exists(true, backup);

            var exception = Assert.Throws<InvalidOperationException>(() => NullBackuper.Default.TryRestore(file));
            StringAssert.IsMatch(@"Expected file .+NullBackuperTests\\TryRestoreWhenHasBackupAndOriginal.cfg to not exist.", exception.Message);

            AssertFile.Exists(true, file);
            AssertFile.Exists(true, softDelete);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void TryRestoreWhenHasBackup()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            var dummy = new DummySerializable(1);
            backup.Save(dummy);

            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);

            Assert.IsFalse(NullBackuper.Default.CanRestore(file));
            Assert.IsFalse(NullBackuper.Default.TryRestore(file));

            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void TryRestoreWhenNoFiles()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);

            Assert.IsFalse(NullBackuper.Default.TryRestore(file));

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
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            using (var lockedFile = LockedFile.CreateIfExists(file, x => x.Open(FileMode.Open, FileAccess.Read, FileShare.Delete)))
            {
                NullBackuper.Default.AfterSave(lockedFile);
            }

            AssertFile.Exists(false, backupOneMinuteOld);
            AssertFile.Exists(false, backupOneHourOld);
            AssertFile.Exists(false, backupOneDayOld);
            AssertFile.Exists(false, backupOneMonthOld);
            AssertFile.Exists(false, backupOneYearOld);
            AssertFile.Exists(false, softDelete);
        }

        [Test]
        public void PurgeWhenHasSoftDelete()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            file.CreateFileOnDisk();
            softDelete.CreateFileOnDisk();
            foreach (var timestamped in timestampedBackups)
            {
                timestamped.CreateFileOnDisk();
            }

            using (var lockedFile = LockedFile.CreateIfExists(file, x => x.Open(FileMode.Open, FileAccess.Read, FileShare.Delete)))
            {
                NullBackuper.Default.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, file);
            AssertFile.Exists(true, backupOneMinuteOld);
            AssertFile.Exists(true, backupOneHourOld);
            AssertFile.Exists(true, backupOneDayOld);
            AssertFile.Exists(true, backupOneMonthOld);
            AssertFile.Exists(true, backupOneYearOld);
            AssertFile.Exists(false, softDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletesNoTimestamp()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            file.CreateFileOnDisk();
            softDelete.CreateFileOnDisk();
            backup.CreateFileOnDisk();
            using (var lockedFile = LockedFile.CreateIfExists(file, x => x.Open(FileMode.Open, FileAccess.Read, FileShare.Delete)))
            {
                NullBackuper.Default.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, file);
            AssertFile.Exists(true, backup);
            AssertFile.Exists(false, softDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletes()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.GetSoftDeleteFileFor();
            foreach (var timestamped in timestampedBackups)
            {
                timestamped.CreateFileOnDisk();
            }

            softDelete.CreateFileOnDisk();
            file.CreateFileOnDisk();
            backup.CreateFileOnDisk();
            using (var lockedFile = LockedFile.CreateIfExists(file, x => x.Open(FileMode.Open, FileAccess.Read, FileShare.Delete)))
            {
                NullBackuper.Default.AfterSave(lockedFile);
            }

            AssertFile.Exists(true, file);
            AssertFile.Exists(true, backup);
            AssertFile.Exists(false, softDelete);
        }

        [Test]
        public void Rename()
        {
            Assert.Inconclusive("Backups must be renamed when original file is renamed");
        }

        private FileInfo CreateFile([CallerMemberName] string name = null) => directory.CreateFileInfoInDirectory(name + ".cfg");

        private FileInfo CreateBackupFile([CallerMemberName] string name = null) => directory.CreateFileInfoInDirectory(name + ".bak");
    }
}
