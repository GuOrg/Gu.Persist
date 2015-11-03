namespace Gu.Settings.Core.Tests.IO
{
    using System.IO;
    using NUnit.Framework;

    public class DirectoryInfoExtTests
    {
        [TestCase(@"c:\", @"c:\", true)]
        [TestCase(@"c:\foo", @"c:\", true)]
        [TestCase(@"c:\foo", @"c:\foo", true)]
        [TestCase(@"c:\foo", @"c:\foo\", true)]
        [TestCase(@"c:\foo\", @"c:\foo", true)]
        [TestCase(@"c:\foo\bar\", @"c:\foo\", true)]
        [TestCase(@"c:\foo\bar", @"c:\foo\", true)]
        [TestCase(@"c:\foo\a.txt", @"c:\foo", true)]
        [TestCase(@"c:\FOO\a.txt", @"c:\foo", true)]
        [TestCase(@"c:/foo/a.txt", @"c:\foo", true)]
        [TestCase(@"c:\foobar", @"c:\foo", false)]
        [TestCase(@"c:\foobar\a.txt", @"c:\foo", false)]
        [TestCase(@"c:\foobar\a.txt", @"c:\foo\", false)]
        [TestCase(@"c:\foo\a.txt", @"c:\foobar", false)]
        [TestCase(@"c:\foo\a.txt", @"c:\foobar\", false)]
        [TestCase(@"c:\foo\..\bar\baz", @"c:\foo", false)]
        [TestCase(@"c:\foo\..\bar\baz", @"c:\bar", true)]
        [TestCase(@"c:\foo\..\bar\baz", @"c:\barr", false)]
        public void IsSubDirectoryOfOrSame(string path, string baseDirPath, bool expected)
        {
            var d1 = new DirectoryInfo(path);
            var d2 = new DirectoryInfo(baseDirPath);
            Assert.AreEqual(expected, d1.IsSubDirectoryOfOrSame(d2));
        }
    }
}