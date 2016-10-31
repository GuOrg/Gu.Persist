namespace Gu.Persist.Core.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    using Gu.Persist.Core;

    public static class RepositoryExt
    {
        private static readonly FieldInfo RepositoryCacheField = typeof(SingletonRepository<>).GetField("cache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
        private static readonly FieldInfo TrackerCacheField = typeof(DirtyTracker).GetField("cache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

        public static ConcurrentDictionary<string, WeakReference> GetCache(this ISingletonRepository repo)
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
            var cache = (repository as ISingletonRepository)?.GetCache();
            cache?.Clear(); // nUnit does some optimization keeping this alive
        }
    }
}
