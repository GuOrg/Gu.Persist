namespace Gu.Persist.Core.Tests.Repositories
{
    using Gu.Persist.Core;

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
            Assert.IsTrue(fileCache.TryGetValue(getKey, out object actual));
            Assert.AreSame(expected, actual);
        }

        [TestCase("key1", "Key2")]
        public void Rename(string fromKey, string toKey)
        {
            var fileCache = new FileCache();
            var expected = new object();
            fileCache.Add(fromKey, expected);
            fileCache.ChangeKey(fromKey, toKey, overWrite: true);
            Assert.IsTrue(fileCache.TryGetValue(toKey, out object actual));
            Assert.AreSame(expected, actual);
        }
    }
}
