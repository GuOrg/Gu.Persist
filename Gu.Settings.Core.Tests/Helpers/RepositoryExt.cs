namespace Gu.Settings.Core.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    using Gu.Settings.Core;

    public static class RepositoryExt
    {
        private static readonly FieldInfo RepositoryCacheField = typeof(Repository<>).GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
        private static readonly FieldInfo TrackerCacheField = typeof(DirtyTracker).GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

        public static ConcurrentDictionary<string, WeakReference> GetCache(this IRepository repo)
        {
            var cache = (ConcurrentDictionary<string, WeakReference>)RepositoryCacheField.GetValue(repo);
            return cache;
        }

        public static ConcurrentDictionary<string, object> GetCache(this IDirtyTracker tracker)
        {
            var cache = (ConcurrentDictionary<string, object>)TrackerCacheField.GetValue(tracker);
            return cache;
        }

        public static void ClearCache(this IRepository repository)
        {
            var cache = repository.GetCache();
            cache.Clear(); // nUnit does some optimization keeping this alive
        }
    }
}
