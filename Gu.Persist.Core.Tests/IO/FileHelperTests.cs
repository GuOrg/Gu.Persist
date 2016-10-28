namespace Gu.Persist.Core.Tests.IO
{
    using System.IO;
    using System.Threading.Tasks;
    using Gu.Persist.Core;

    using NUnit.Framework;

    public class FileHelperTests
    {
        private readonly DirectoryInfo Directory;
        private FileInfo file;
        private FileInfo softDeleteFile;
        private FileInfo backup;
        private FileInfo backupSoftDelete;

        public FileHelperTests()
        {
            this.Directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name);
            this.Directory.CreateIfNotExists();
        }

        [SetUp]
        public void SetUp()
        {
            this.file = this.Directory.CreateFileInfoInDirectory("Setting.cfg");
            this.softDeleteFile = this.file.GetSoftDeleteFileFor();
            this.backup = this.file.WithNewExtension(BackupSettings.DefaultExtension);
            this.backupSoftDelete = this.backup.GetSoftDeleteFileFor();
            this.backup.VoidCreate();
        }

        [TearDown]
        public void TearDown()
        {
            this.file.Delete();
            this.backup.Delete();
            this.softDeleteFile.Delete();
        }

        [Test]
        public void HardDeleteWhenNoFile()
        {
            this.file.HardDelete();
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(false, this.softDeleteFile);
            AssertFile.Exists(true, this.backup);
        }

        [Test]
        public void HardDeleteWhenNoSoftFile()
        {
            this.file.VoidCreate();
            this.file.HardDelete();
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(false, this.softDeleteFile);
            AssertFile.Exists(true, this.backup);
        }

        [Test]
        public void HardDeleteWhenHasSoftFile()
        {
            this.file.VoidCreate();
            this.softDeleteFile.VoidCreate();
            this.file.HardDelete();
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(false, this.softDeleteFile);
            AssertFile.Exists(true, this.backup);
        }

        [Test]
        public void HardDeleteWhenOnlySoftFile()
        {
            this.softDeleteFile.VoidCreate();
            this.file.HardDelete();
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(false, this.softDeleteFile);
            AssertFile.Exists(true, this.backup);
        }

        [Test]
        public void SoftDeleteWhenNoFile()
        {
            this.file.SoftDelete();
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(false, this.softDeleteFile);
            AssertFile.Exists(true, this.backup);
        }

        [Test]
        public void SoftDeleteWhenNoSoftFile()
        {
            this.file.VoidCreate();
            this.file.SoftDelete();
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(true, this.softDeleteFile);
            AssertFile.Exists(true, this.backup);
        }

        [Test]
        public void SoftDeleteWhenHasSoftFile()
        {
            this.file.WriteAllText("File");
            this.softDeleteFile.WriteAllText("Soft");
            this.file.SoftDelete();
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(true, this.softDeleteFile);
            Assert.AreEqual("File", this.softDeleteFile.ReadAllText());
            AssertFile.Exists(true, this.backup);
        }

        [Test]
        public void BackupWhenNoFile()
        {
            this.backup.Delete();
            AssertFile.Exists(false, this.file);
            FileHelper.Backup(this.file, this.backup);
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(false, this.backup);
        }

        [Test]
        public void BackupWhenNoFileButHasBackup()
        {
            this.backup.WriteAllText("Backup");
            AssertFile.Exists(false, this.file);
            FileHelper.Backup(this.file, this.backup);
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(true, this.backup);
            Assert.AreEqual("Backup", this.backup.ReadAllText());
        }

        [Test]
        public void BackupWhenNoBackupFile()
        {
            this.file.WriteAllText("File");
            FileHelper.Backup(this.file, this.backup);
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(true, this.backup);
            Assert.AreEqual("File", this.backup.ReadAllText());
        }

        [Test]
        public void BackupWhenHasBackupFile()
        {
            this.file.WriteAllText("File");
            this.backup.WriteAllText("Backup");
            FileHelper.Backup(this.file, this.backup);
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(true, this.backup);
            Assert.AreEqual("File", this.backup.ReadAllText());
        }

        [Test]
        public void BackupWhenHasBackupFileAndBackupHasSoftDelete()
        {
            this.backupSoftDelete.WriteAllText("OldSoft");
            this.file.WriteAllText("File");
            this.backup.WriteAllText("Backup");
            FileHelper.Backup(this.file, this.backup);
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(true, this.backup);
            Assert.AreEqual("File", this.backup.ReadAllText());
            AssertFile.Exists(true, this.backupSoftDelete);
            Assert.AreEqual("Backup", this.backupSoftDelete.ReadAllText());
        }

        [Test]
        public void RestoreWhenNoFile()
        {
            this.backup.Delete();
            AssertFile.Exists(false, this.file);
            this.file.Restore(this.backup);
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(false, this.backup);
        }

        [Test]
        public void RestoreWhenSoftDeleteFile()
        {
            this.softDeleteFile.WriteAllText("Soft");
            this.backup.Delete();
            AssertFile.Exists(false, this.file);
            this.file.Restore(this.softDeleteFile);
            AssertFile.Exists(true, this.file);
            Assert.AreEqual("Soft", this.file.ReadAllText());
            AssertFile.Exists(false, this.backup);
            AssertFile.Exists(false, this.softDeleteFile);
        }

        [Test]
        public void RestoreWhenNoRestoreFile()
        {
            this.backup.Delete();
            this.file.WriteAllText("File");
            this.file.Restore(this.backup);
            AssertFile.Exists(true, this.file);
            AssertFile.Exists(false, this.backup);
            Assert.AreEqual("File", this.file.ReadAllText());
        }

        [Test]
        public void RestoreWhenHasRestoreFile()
        {
            this.file.WriteAllText("File");
            this.backup.WriteAllText("Restore");
            this.file.Restore(this.backup);
            AssertFile.Exists(true, this.file);
            AssertFile.Exists(false, this.backup);
            Assert.AreEqual("Restore", this.file.ReadAllText());
        }

        [Test]
        public void SoftDeleteWhenOnlySoftFile()
        {
            this.softDeleteFile.VoidCreate();
            this.file.SoftDelete();
            AssertFile.Exists(false, this.file);
            AssertFile.Exists(true, this.softDeleteFile);
        }

        [TestCase(@"C:\Temp", "Setting", "cfg", @"C:\Temp\Setting.cfg")]
        [TestCase(@"C:\Temp", "Setting.xml", ".cfg", @"C:\Temp\Setting.cfg")]
        [TestCase(null, @"C:\Temp\Setting.cfg", null, @"C:\Temp\Setting.cfg")]
        [TestCase(null, @"C:\Temp\Setting.xml", "cfg", @"C:\Temp\Setting.cfg")]
        public void CreateFileInfo(string dir, string fn, string ext, string expected)
        {
            if (dir != null)
            {
                var directory = new DirectoryInfo(dir);
                var fileInfo = FileHelper.CreateFileInfo(directory, fn, ext);
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
            var fileInfo = this.Directory.CreateFileInfoInDirectory("SaveAsyncTest.cfg");
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("1 2");
                writer.Flush();
                stream.Position = 0;
                await FileHelper.SaveAsync(fileInfo, stream).ConfigureAwait(false);
            }

            var text = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("1 2", text);

            stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("3");
                writer.Flush();
                stream.Position = 0;
                await FileHelper.SaveAsync(fileInfo, stream).ConfigureAwait(false);
            }

            text = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("3", text);
        }
    }
}