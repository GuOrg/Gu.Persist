namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    using NUnit.Framework;

    public class FileHelperTests
    {
        [TestCase(".old")]
        [TestCase("old")]
        public void TestNameTest(string extension)
        {
            var fileInfo = new FileInfo(@"C:\Temp\Setting.cfg");
            var backupFileName = FileHelper.BackupFileName(fileInfo, extension);
            Assert.AreEqual(@"C:\Temp\Setting.old", backupFileName.FullName);
        }
    }
}
