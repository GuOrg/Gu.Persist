namespace Gu.Settings.Core.Tests.IO
{
    using System;
    using System.IO;
    using Gu.Settings.Core;

    using NUnit.Framework;

    public class DirectoriesTests
    {
        [Test]
        public void Default()
        {
            var directoryInfo = Directories.Default;
            Console.WriteLine(directoryInfo.FullName);
        }

        [Test]
        public void MyDocuments()
        {
            var directoryInfo = Directories.MyDocuments;
            Console.WriteLine(directoryInfo.FullName);
        }

        [Test]
        public void CreateInfoRooted()
        {
            var path = @"C:\Temp";
            var info = Directories.CreateInfo(path);
            Assert.AreEqual(path, info.FullName);
            var actual = Directories.GetPath(info);
            Assert.AreEqual(path, actual);
        }

        [Test]
        public void RoundtripAppData()
        {
            var path = @"%AppData%";
            var info = Directories.CreateInfo(path);
            var expected = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Assert.AreEqual(expected, info.FullName);
            var actual = Directories.GetPath(info);
            Assert.AreEqual(path, actual);
        }

        [Test]
        public void RoundtripAppDataSubfolder()
        {
            var path = @"%AppData%\Test";
            var info = Directories.CreateInfo(path);
            var expected = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Test");
            Assert.AreEqual(expected, info.FullName);
            var actual = Directories.GetPath(info);
            Assert.AreEqual(path, actual);
        }

        [Test]
        public void RoundtripCurrentDirectory()
        {
            var path = @".";
            var info = Directories.CreateInfo(path);
            Assert.AreEqual(Environment.CurrentDirectory, info.FullName);
            var actual = Directories.GetPath(info);
            Assert.AreEqual(path, actual);
        }

        [Test]
        public void RoundtripCurrentDirectorySub()
        {
            var path = @".\Settings";
            var info = Directories.CreateInfo(path);
            Assert.AreEqual(Environment.CurrentDirectory + "\\Settings", info.FullName);
            var actual = Directories.GetPath(info);
            Assert.AreEqual(path, actual);
        }
    }
}
