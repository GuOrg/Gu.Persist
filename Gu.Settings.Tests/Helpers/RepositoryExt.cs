namespace Gu.Settings.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Reflection;

    public static class RepositoryExt
    {
        public static void ClearCache(this IRepository repository)
        {
            var cacheField = typeof(Repository).GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            var cache = (ConcurrentDictionary<FileInfo, WeakReference>)cacheField.GetValue(repository);
            cache.Clear(); // nUnit does some optimization keeping this alive
        }
    }
}
