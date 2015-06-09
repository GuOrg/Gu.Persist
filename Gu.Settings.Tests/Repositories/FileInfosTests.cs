namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    using NUnit.Framework;

    public class FileInfosTests
    {
        [Test]
        public void CreateFileInfosWithBackup()
        {
            var fileInfos = FileInfos.CreateFileInfos(new DirectoryInfo(@"C:\Temp"), "Setting", true, ".cfg", ".old");
            Assert.AreEqual(@"C:\Temp\Setting.cfg", fileInfos.File.FullName);
            Assert.AreEqual(@"C:\Temp\Setting.old", fileInfos.Backup.FullName);
        }
    }
}
