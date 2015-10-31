namespace Gu.Settings.Core.Tests.Settings
{
    using System;
    using System.IO;

    using NUnit.Framework;

    public class PathAndSpecialFolderTests
    {
        [Test]
        public void Default()
        {
            Assert.Fail("Test this");
        }

        [Test]
        public void AbsolutePath()
        {
            var path = @"C:\Temp";
            var pathAndSpecialFolder = PathAndSpecialFolder.Create(path);
            var info = pathAndSpecialFolder.CreateDirectoryInfo();
            Assert.AreEqual(path, info.FullName);
            var actual = PathAndSpecialFolder.Create(info);
            Assert.AreEqual(path, actual.Path);
            Assert.AreEqual(null, actual.SpecialFolder);
        }

        [Test]
        public void CreateAppDataWithoutSubDirThrows()
        {
            var path = Environment.ExpandEnvironmentVariables(@"%AppData%");
            Assert.Throws<InvalidOperationException>(() => PathAndSpecialFolder.Create(path));
        }

        [Test]
        public void RoundtripAppDataSubfolder()
        {
            var path = @"%AppData%\Test";
            var info = Directories.CreateInfo(path);
            var expected = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Test");
            Assert.AreEqual(expected, info.FullName);
            var actual = PathAndSpecialFolder.Create(info);
            Assert.AreEqual(@"\Test", actual.Path);
            Assert.AreEqual(Environment.SpecialFolder.ApplicationData, actual.SpecialFolder);
        }

        [Test]
        public void RoundtripCurrentDirectory()
        {
            var path = @".";
            var pathAndSpecialFolder = PathAndSpecialFolder.Create(path);
            var info = pathAndSpecialFolder.CreateDirectoryInfo();
            Assert.AreEqual(Environment.CurrentDirectory, info.FullName);
            var actual = PathAndSpecialFolder.Create(info);
            Assert.AreEqual(path, actual.Path);
            Assert.AreEqual(null, actual.SpecialFolder);
        }

        [Test]
        public void RoundtripCurrentDirectorySub()
        {
            var path = @".\Settings";
            var pathAndSpecialFolder = PathAndSpecialFolder.Create(path);
            var info = pathAndSpecialFolder.CreateDirectoryInfo();
            Assert.AreEqual(Environment.CurrentDirectory + "\\Settings", info.FullName);
            var actual = PathAndSpecialFolder.Create(info);
            Assert.AreEqual(path, actual.Path);
            Assert.AreEqual(null, actual.SpecialFolder);
        }

        [TestCase(@"c:\", @"c:\", true)]
        [TestCase(@"c:\foo", @"c:\", true)]
        [TestCase(@"c:\foo", @"c:\foo", true)]
        [TestCase(@"c:\foo", @"c:\foo\", true)]
        [TestCase(@"c:\foo\", @"c:\foo", true)]
        [TestCase(@"c:\foo\bar\", @"c:\foo\", true)]
        [TestCase(@"c:\foo\bar", @"c:\foo\", true)]
        [TestCase(@"c:\foobar", @"c:\foo", false)]
        [TestCase(@"c:\foo\..\bar\baz", @"c:\foo", false)]
        [TestCase(@"c:\foo\..\bar\baz", @"c:\bar", true)]
        [TestCase(@"c:\foo\..\bar\baz", @"c:\barr", false)]
        public void IsSubDirectoryOfOrSame(string path, string potentialParent, bool expected)
        {
            Assert.AreEqual(expected, PathAndSpecialFolder.IsSubDirectoryOfOrSame(path, potentialParent));
        }
    }
}