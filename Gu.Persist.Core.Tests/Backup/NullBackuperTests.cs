// ReSharper disable AssignNullToNotNullAttribute
#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Backup
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    public static class NullBackuperTests
    {
        private static readonly DirectoryInfo Directory = Directories.TempDirectory
                                                                     .CreateSubdirectory("Gu.Persist.Tests")
                                                                     .CreateSubdirectory(typeof(NullBackuperTests).FullName!);

        [SetUp]
        public static void SetUp()
        {
            _ = Directory.CreateIfNotExists();
        }

        [TearDown]
        public static void TearDown()
        {
            Directory.DeleteIfExists(true);
        }

        [Test]
        public static void BackupWhenNotExists()
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
        public static void TryRestoreWhenHasSoftDelete()
        {
            var dummy = new DummySerializable(1);
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.SoftDeleteFile();
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
        public static void TryRestoreWhenHasSoftDeleteAndBackup()
        {
            var dummy = new DummySerializable(1);
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.SoftDeleteFile();
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
        public static void TryRestoreWhenHasBackupAndOriginal()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.SoftDeleteFile();
            file.WriteAllText("File");
            softDelete.CreateFileOnDisk();
            backup.CreateFileOnDisk();

            AssertFile.Exists(true, file);
            AssertFile.Exists(true, softDelete);
            AssertFile.Exists(true, backup);

            var exception = Assert.Throws<InvalidOperationException>(() => NullBackuper.Default.TryRestore(file));
            StringAssert.IsMatch(@"Expected file .+NullBackuperTests\\TryRestoreWhenHasBackupAndOriginal.cfg to not exist.", exception!.Message);

            AssertFile.Exists(true, file);
            AssertFile.Exists(true, softDelete);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public static void TryRestoreWhenHasBackup()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.SoftDeleteFile();
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
        public static void TryRestoreWhenNoFiles()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            var softDelete = file.SoftDeleteFile();
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDelete);
            AssertFile.Exists(false, backup);

            Assert.IsFalse(NullBackuper.Default.TryRestore(file));

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
            var softDelete = file.SoftDeleteFile();
            using (var lockedFile = LockedFile.CreateIfExists(file, x => x.Open(FileMode.Open, FileAccess.Read, FileShare.Delete)))
            {
                NullBackuper.Default.AfterSave(lockedFile!);
            }

            AssertFile.Exists(false, softDelete);
        }

        [Test]
        public static void AfterSaveWhenHasSoftDelete()
        {
            var file = CreateFile();
            var softDelete = file.SoftDeleteFile();
            file.CreateFileOnDisk();
            softDelete.CreateFileOnDisk();
            using (var lockedFile = LockedFile.CreateIfExists(file, x => x.Open(FileMode.Open, FileAccess.Read, FileShare.Delete)))
            {
                NullBackuper.Default.AfterSave(lockedFile!);
            }

            AssertFile.Exists(true, file);
            AssertFile.Exists(false, softDelete);
        }

        [Test]
        public static void Rename()
        {
            Assert.Inconclusive("Backups must be renamed when original file is renamed");
        }

        private static FileInfo CreateFile([CallerMemberName] string? name = null) => Directory.CreateFileInfoInDirectory(name + ".cfg");

        private static FileInfo CreateBackupFile([CallerMemberName] string? name = null) => Directory.CreateFileInfoInDirectory(name + ".bak");
    }
}
