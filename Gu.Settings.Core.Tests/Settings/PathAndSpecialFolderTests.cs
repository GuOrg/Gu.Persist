namespace Gu.Settings.Core.Tests.Settings
{
    using System;
    using System.IO;

    using NUnit.Framework;

    public class PathAndSpecialFolderTests
    {
        [Test]
        public void TestNameTest()
        {
            var directoryInfo = new DirectoryInfo(".");
            Console.WriteLine(directoryInfo.FullName);
        }

        [Test]
        public void Default()
        {
            var pathAndSpecialFolder = PathAndSpecialFolder.Default;
            Assert.AreEqual(Environment.SpecialFolder.ApplicationData, pathAndSpecialFolder.SpecialFolder);
            Assert.AreEqual("Gu.Settings.Core.Tests", pathAndSpecialFolder.Path);
            var expected = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Gu.Settings.Core.Tests");
            Assert.AreEqual(expected, pathAndSpecialFolder.CreateDirectoryInfo().FullName);
        }

        [Test]
        public void DefaultFor()
        {
            var pathAndSpecialFolder = PathAndSpecialFolder.DefaultFor(GetType().Assembly);
            Assert.AreEqual(Environment.SpecialFolder.ApplicationData, pathAndSpecialFolder.SpecialFolder);
            Assert.AreEqual("Gu.Settings.Core.Tests", pathAndSpecialFolder.Path);
            var expected = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Gu.Settings.Core.Tests");
            Assert.AreEqual(expected, pathAndSpecialFolder.CreateDirectoryInfo().FullName);
        }

        [Test]
        public void AbsolutePath()
        {
            var path = @"C:\Foo";
            var pathAndSpecialFolder = PathAndSpecialFolder.Create(path);
            var info = pathAndSpecialFolder.CreateDirectoryInfo();
            Assert.AreEqual(path, info.FullName);
            var actual = PathAndSpecialFolder.Create(info);
            Assert.AreEqual(path, actual.Path);
            Assert.AreEqual(null, actual.SpecialFolder);
        }

        [Test]
        public void AppDataWithoutSubDirThrows()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Assert.Throws<ArgumentException>(() => new PathAndSpecialFolder(path, null));
            Assert.Throws<ArgumentException>(() => new PathAndSpecialFolder(path, Environment.SpecialFolder.ApplicationData));
            Assert.Throws<ArgumentException>(() => new PathAndSpecialFolder("", Environment.SpecialFolder.ApplicationData));
            Assert.Throws<ArgumentException>(() => new PathAndSpecialFolder(null, Environment.SpecialFolder.ApplicationData));
            Assert.Throws<ArgumentException>(() => PathAndSpecialFolder.Create(path));
            Assert.Throws<ArgumentException>(() => PathAndSpecialFolder.Create(new DirectoryInfo(path)));
        }

        [Test]
        public void AppDataSubfolder()
        {
            var path = Environment.ExpandEnvironmentVariables(@"%AppData%\Test");
            var actuals = new[]
                              {
                                  PathAndSpecialFolder.Create(path),
                                  PathAndSpecialFolder.Create(new DirectoryInfo(path)),
                                  new PathAndSpecialFolder(path, Environment.SpecialFolder.ApplicationData),
                                  new PathAndSpecialFolder("Test", Environment.SpecialFolder.ApplicationData),
                                  new PathAndSpecialFolder(@"\Test", Environment.SpecialFolder.ApplicationData),
                                  new PathAndSpecialFolder(path, null),
                              };
            foreach (var actual in actuals)
            {
                Assert.AreEqual(@"\Test", actual.Path);
                Assert.AreEqual(Environment.SpecialFolder.ApplicationData, actual.SpecialFolder);
                var info = actual.CreateDirectoryInfo();
                Assert.AreEqual(path, info.FullName);
            }
        }

        [Test]
        public void CurrentDirectory()
        {
            var path = @".";
            var actuals = new[]
                              {
                                  PathAndSpecialFolder.Create(path),
                                  PathAndSpecialFolder.Create(new DirectoryInfo(Environment.CurrentDirectory)),
                                  new PathAndSpecialFolder(path, null),
                                  new PathAndSpecialFolder(Environment.CurrentDirectory, null),
                              };
            foreach (var actual in actuals)
            {
                Assert.AreEqual(@".", actual.Path);
                Assert.AreEqual(null, actual.SpecialFolder);
                var info = actual.CreateDirectoryInfo();
                Assert.AreEqual(Environment.CurrentDirectory, info.FullName);
            }
        }

        [Test]
        public void CurrentDirectorySubDirectory()
        {
            var path = @".\Settings";
            var actuals = new[]
                              {
                                  PathAndSpecialFolder.Create(path),
                                  PathAndSpecialFolder.Create(new DirectoryInfo(Environment.CurrentDirectory +"\\Settings")),
                                  new PathAndSpecialFolder(path, null),
                              };
            foreach (var actual in actuals)
            {
                var info = actual.CreateDirectoryInfo();
                Assert.AreEqual(Environment.CurrentDirectory + "\\Settings", info.FullName);
                Assert.AreEqual(path, actual.Path);
                Assert.AreEqual(null, actual.SpecialFolder);
            }
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