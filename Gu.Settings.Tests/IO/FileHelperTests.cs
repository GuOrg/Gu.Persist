namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    using NUnit.Framework;

    public class FileHelperTests
    {
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
    }
}