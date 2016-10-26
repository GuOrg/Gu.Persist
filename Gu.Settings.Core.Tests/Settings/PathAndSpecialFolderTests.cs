namespace Gu.Settings.Core.Tests.Settings
{
    using System;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;

    public class PathAndSpecialFolderTests
    {
        [Test]
        public void Default()
        {
            var pathAndSpecialFolder = PathAndSpecialFolder.Default;
            Assert.AreEqual(Environment.SpecialFolder.ApplicationData, pathAndSpecialFolder.SpecialFolder);
            
            // This is needed if R# shadow copies
            var directoryName = Path.GetFileName(Directory.GetCurrentDirectory());
            Assert.AreEqual(directoryName, pathAndSpecialFolder.Path);
            var expected = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), directoryName);
            Assert.AreEqual(expected, pathAndSpecialFolder.CreateDirectoryInfo().FullName);
        }

        [Test]
        public void DefaultFor()
        {
            var pathAndSpecialFolder = PathAndSpecialFolder.DefaultFor(this.GetType().Assembly);
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
                Assert.AreEqual("Test", actual.Path);
                Assert.AreEqual(Environment.SpecialFolder.ApplicationData, actual.SpecialFolder);
                var info = actual.CreateDirectoryInfo();
                Assert.AreEqual(path, info.FullName);
            }
        }

        [Test]
        public void AppDataNestedSubfolder()
        {
            var path = Environment.ExpandEnvironmentVariables(@"%AppData%\Level1\Level2");
            var actuals = new[]
                              {
                                  PathAndSpecialFolder.Create(path),
                                  PathAndSpecialFolder.Create(new DirectoryInfo(path)),
                                  new PathAndSpecialFolder(path, Environment.SpecialFolder.ApplicationData),
                                  new PathAndSpecialFolder(@"Level1\Level2", Environment.SpecialFolder.ApplicationData),
                                  new PathAndSpecialFolder(@"\Level1\Level2", Environment.SpecialFolder.ApplicationData),
                                  new PathAndSpecialFolder(path, null),
                              };
            foreach (var actual in actuals)
            {
                Assert.AreEqual(@"Level1\Level2", actual.Path);
                Assert.AreEqual(Environment.SpecialFolder.ApplicationData, actual.SpecialFolder);
                var info = actual.CreateDirectoryInfo();
                Assert.AreEqual(path, info.FullName);
            }
        }

        [Test]
        public void CurrentDirectory()
        {
            var path = @".";
            var fullName = Environment.CurrentDirectory;
            var actuals = new[]
                              {
                                  PathAndSpecialFolder.Create(path),
                                  PathAndSpecialFolder.Create(new DirectoryInfo(fullName)),
                                  new PathAndSpecialFolder(path, null),
                                  new PathAndSpecialFolder(fullName, null),
                              };
            foreach (var actual in actuals)
            {
                Assert.AreEqual(@".", actual.Path);
                Assert.AreEqual(null, actual.SpecialFolder);
                var info = actual.CreateDirectoryInfo();
                Assert.AreEqual(fullName, info.FullName);
            }
        }

        [Test]
        public void CurrentDirectorySubDirectory()
        {
            var path = @".\Settings";
            var fullName = System.IO.Path.Combine(Environment.CurrentDirectory, "Settings");
            var actuals = new[]
                              {
                                  PathAndSpecialFolder.Create(path),
                                  PathAndSpecialFolder.Create(new DirectoryInfo(fullName)),
                                  new PathAndSpecialFolder(path, null),
                                  new PathAndSpecialFolder(fullName, null),
                              };
            foreach (var actual in actuals)
            {
                Assert.AreEqual(path, actual.Path);
                Assert.AreEqual(null, actual.SpecialFolder);
                var info = actual.CreateDirectoryInfo();
                Assert.AreEqual(fullName, info.FullName);
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
        //[TestCase(@"c:\foo\..\bar\baz", @"c:\foo", false)]
        //[TestCase(@"c:\foo\..\bar\baz", @"c:\bar", true)]
        //[TestCase(@"c:\foo\..\bar\baz", @"c:\barr", false)]
        public void IsSubDirectoryOfOrSame(string path, string potentialParent, bool expected)
        {
            var method = typeof(PathAndSpecialFolder).GetMethod("IsSubDirectoryOfOrSame",
                BindingFlags.Static | BindingFlags.NonPublic);

            Assert.AreEqual(expected, method.Invoke(null, new[] { path, potentialParent }));
        }
    }
}