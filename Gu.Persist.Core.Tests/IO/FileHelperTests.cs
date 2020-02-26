// ReSharper disable AssignNullToNotNullAttribute
#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.IO
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Gu.Persist.Core;

    using NUnit.Framework;

    public class FileHelperTests
    {
        private readonly DirectoryInfo directory;

        public FileHelperTests()
        {
            this.directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                        .CreateSubdirectory(this.GetType().FullName);
            _ = this.directory.CreateIfNotExists();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.directory.Delete(true);
        }

        [Test]
        public void HardDeleteWhenNoFile()
        {
            var file = this.CreateFileInfo();
            var softDeleteFile = file.SoftDeleteFile();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            file.HardDelete();
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDeleteFile);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void HardDeleteWhenNoSoftFile()
        {
            var file = this.CreateFileInfo();
            var softDeleteFile = file.SoftDeleteFile();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            file.CreateFileOnDisk();
            file.HardDelete();
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDeleteFile);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void HardDeleteWhenHasSoftFile()
        {
            var file = this.CreateFileInfo();
            var softDeleteFile = file.SoftDeleteFile();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            file.CreateFileOnDisk();
            softDeleteFile.CreateFileOnDisk();
            file.HardDelete();
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDeleteFile);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void HardDeleteWhenOnlySoftFile()
        {
            var file = this.CreateFileInfo();
            var softDeleteFile = file.SoftDeleteFile();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            softDeleteFile.CreateFileOnDisk();
            file.HardDelete();
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDeleteFile);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void SoftDeleteWhenNoFile()
        {
            var file = this.CreateFileInfo();
            var softDeleteFile = file.SoftDeleteFile();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            var soft = file.SoftDelete();
            Assert.AreEqual(null, soft);
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, softDeleteFile);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void SoftDeleteWhenNoSoftFile()
        {
            var file = this.CreateFileInfo();
            var softDeleteFile = file.SoftDeleteFile();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            file.CreateFileOnDisk();
            var soft = file.SoftDelete();
            Assert.AreEqual(soft.FullName, softDeleteFile.FullName);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, softDeleteFile);
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void SoftDeleteWhenHasSoftFile()
        {
            var file = this.CreateFileInfo();
            var softDeleteFile = file.SoftDeleteFile();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            file.WriteAllText("File");
            softDeleteFile.WriteAllText("Soft");
            var soft = file.SoftDelete();
            Assert.AreEqual(soft.FullName, softDeleteFile.FullName);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, softDeleteFile);
            Assert.AreEqual("File", softDeleteFile.ReadAllText());
            AssertFile.Exists(true, backup);
        }

        [Test]
        public void BackupWhenNoFile()
        {
            var file = this.CreateFileInfo();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            backup.Delete();
            AssertFile.Exists(false, file);
            FileHelper.Backup(file, backup);
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public void BackupWhenNoFileButHasBackup()
        {
            var file = this.CreateFileInfo();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            backup.WriteAllText("Backup");
            AssertFile.Exists(false, file);
            FileHelper.Backup(file, backup);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, backup);
            Assert.AreEqual("Backup", backup.ReadAllText());
        }

        [Test]
        public void BackupWhenNoBackupFile()
        {
            var file = this.CreateFileInfo();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            file.WriteAllText("File");
            FileHelper.Backup(file, backup);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, backup);
            Assert.AreEqual("File", backup.ReadAllText());
        }

        [Test]
        public void BackupWhenHasBackupFile()
        {
            var file = this.CreateFileInfo();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            file.WriteAllText("File");
            backup.WriteAllText("Backup");
            FileHelper.Backup(file, backup);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, backup);
            Assert.AreEqual("File", backup.ReadAllText());
        }

        [Test]
        public void BackupWhenHasBackupFileAndBackupHasSoftDelete()
        {
            var file = this.CreateFileInfo();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            var backupSoftDelete = backup.SoftDeleteFile();
            backup.CreateFileOnDisk();
            backupSoftDelete.WriteAllText("OldSoft");
            file.WriteAllText("File");
            backup.WriteAllText("Backup");
            FileHelper.Backup(file, backup);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, backup);
            Assert.AreEqual("File", backup.ReadAllText());
            AssertFile.Exists(true, backupSoftDelete);
            Assert.AreEqual("Backup", backupSoftDelete.ReadAllText());
        }

        [Test]
        public void RestoreWhenNoFile()
        {
            var file = this.CreateFileInfo();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            backup.Delete();
            AssertFile.Exists(false, file);
            file.Restore(backup);
            AssertFile.Exists(false, file);
            AssertFile.Exists(false, backup);
        }

        [Test]
        public void RestoreWhenSoftDeleteFile()
        {
            var file = this.CreateFileInfo();
            var softDeleteFile = file.SoftDeleteFile();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            softDeleteFile.WriteAllText("Soft");
            backup.Delete();
            AssertFile.Exists(false, file);
            file.Restore(softDeleteFile);
            AssertFile.Exists(true, file);
            Assert.AreEqual("Soft", file.ReadAllText());
            AssertFile.Exists(false, backup);
            AssertFile.Exists(false, softDeleteFile);
        }

        [Test]
        public void RestoreWhenNoRestoreFile()
        {
            var file = this.CreateFileInfo();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            backup.Delete();
            file.WriteAllText("File");
            file.Restore(backup);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, backup);
            Assert.AreEqual("File", file.ReadAllText());
        }

        [Test]
        public void RestoreWhenHasRestoreFile()
        {
            var file = this.CreateFileInfo();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            file.WriteAllText("File");
            backup.WriteAllText("Restore");
            file.Restore(backup);
            AssertFile.Exists(true, file);
            AssertFile.Exists(false, backup);
            Assert.AreEqual("Restore", file.ReadAllText());
        }

        [Test]
        public void SoftDeleteWhenOnlySoftFile()
        {
            var file = this.CreateFileInfo();
            var softDeleteFile = file.SoftDeleteFile();
            var backup = file.WithNewExtension(BackupSettings.DefaultExtension);
            backup.CreateFileOnDisk();
            softDeleteFile.CreateFileOnDisk();
            var soft = file.SoftDelete();
            Assert.AreEqual(null, soft);
            AssertFile.Exists(false, file);
            AssertFile.Exists(true, softDeleteFile);
        }

        [TestCase(@"C:\Temp", "Setting", "cfg", @"C:\Temp\Setting.cfg")]
        [TestCase(@"C:\Temp", "Setting.xml", ".cfg", @"C:\Temp\Setting.cfg")]
        [TestCase(null, @"C:\Temp\Setting.cfg", null, @"C:\Temp\Setting.cfg")]
        [TestCase(null, @"C:\Temp\Setting.xml", "cfg", @"C:\Temp\Setting.cfg")]
        public void CreateFileInfo(string dir, string fn, string ext, string expected)
        {
            if (dir != null)
            {
                var directoryInfo = new DirectoryInfo(dir);
                var fileInfo = FileHelper.CreateFileInfo(directoryInfo, fn, ext);
                Assert.AreEqual(expected, fileInfo.FullName);
            }
            else
            {
                var fileInfo = FileHelper.CreateFileInfo(null, fn, ext);
                Assert.AreEqual(expected, fileInfo.FullName);
            }
        }

        [Test]
        public async Task SaveAsync()
        {
            var fileInfo = this.directory.CreateFileInfoInDirectory("SaveAsyncTest.cfg");
            using (var stream = PooledMemoryStream.Borrow())
            {
                using var writer = new StreamWriter(stream);
                writer.Write("1 2");
                writer.Flush();
                stream.Position = 0;
                await FileHelper.SaveAsync(fileInfo, stream).ConfigureAwait(false);
            }

            var text = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("1 2", text);
            using (var stream = PooledMemoryStream.Borrow())
            {
                using var writer = new StreamWriter(stream);
                writer.Write("3");
                writer.Flush();
                stream.Position = 0;
                await FileHelper.SaveAsync(fileInfo, stream).ConfigureAwait(false);
            }

            text = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("3", text);
        }

        private FileInfo CreateFileInfo([CallerMemberName] string name = null) =>
            this.directory.CreateFileInfoInDirectory(name + ".cfg");
    }
}