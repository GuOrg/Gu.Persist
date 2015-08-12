namespace Gu.Settings.Core.Tests.Repositories
{
    using Gu.Settings.Core;

    using NUnit.Framework;

    public class FileCacheTests
    {
        [TestCase("key", "key")]
        [TestCase("key", "Key")]
        public void AddThenGetTest(string addKey, string getKey)
        {
            var fileCache = new FileCache();
            var expected = new object();
            fileCache.Add(addKey, expected);
            object actual;
            Assert.IsTrue(fileCache.TryGetValue(getKey, out actual));
            Assert.AreSame(expected, actual);
        }

        [TestCase("key1", "Key2")]
        public void Rename(string fromKey, string toKey)
        {
            var fileCache = new FileCache();
            var expected = new object();
            fileCache.Add(fromKey, expected);
            fileCache.ChangeKey(fromKey, toKey, true);
            object actual;
            Assert.IsTrue(fileCache.TryGetValue(toKey, out actual));
            Assert.AreSame(expected, actual);
        }
    }
}
