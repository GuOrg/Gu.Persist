namespace Gu.Persist.Core.Tests.Comparers
{
    using System.IO;
    using NUnit.Framework;

    public class DirectoryInfoComparerTests
    {
        [TestCase(@"c:\", @"c:\", true)]
        [TestCase(@"c:\foo", @"c:\foo", true)]
        [TestCase(@"c:\Foo", @"c:\foo", true)]
        [TestCase(@"c:\foo", @"c:\foo\", true)]
        [TestCase(@"c:\foo\bar\..", @"c:\foo\", true)]
        [TestCase(@"c:\foo\bar\", @"c:\foo\", false)]
        [TestCase(@"c:\foobar\", @"c:\foo\", false)]
        public void EqualsAndGetHashCode(string path1, string path2, bool expected)
        {
            var d1 = new DirectoryInfo(path1);
            var d2 = new DirectoryInfo(path2);
            Assert.AreEqual(expected, DirectoryInfoComparer.Default.Equals(d1, d2));
            if (expected)
            {
                Assert.AreEqual(DirectoryInfoComparer.Default.GetHashCode(d1), DirectoryInfoComparer.Default.GetHashCode(d2));
            }
            else
            {
                Assert.AreNotEqual(DirectoryInfoComparer.Default.GetHashCode(d1), DirectoryInfoComparer.Default.GetHashCode(d2));
            }
        }
    }
}
