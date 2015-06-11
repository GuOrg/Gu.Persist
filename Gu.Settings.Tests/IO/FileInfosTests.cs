namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    using NUnit.Framework;

    public class FileInfosTests
    {
        [Test]
        public void CreateFileInfosWithBackup()
        {
            var fileInfo = new FileInfo(@"C:\Temp\Setting.cfg");
            var fileInfos = FileInfos.CreateFileInfos(fileInfo, ".tmp", ".old");
            Assert.AreEqual(@"C:\Temp\Setting.cfg", fileInfos.File.FullName);
            Assert.AreEqual(@"C:\Temp\Setting.tmp", fileInfos.TempFile.FullName);
            Assert.AreEqual(@"C:\Temp\Setting.old", fileInfos.Backup.FullName);
        }
    }
}
