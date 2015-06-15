namespace Gu.Settings.Tests
{
    using System.IO;
    using NUnit.Framework;

    public static class AssertFile
    {
        public static void Exists(bool expected, FileInfo file)
        {
            file.Refresh();
            Assert.AreEqual(expected, file.Exists, "Expected File.Exists == {0} for {1}", expected, file.FullName);
        }
    }
}
