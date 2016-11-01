namespace Gu.Persist.Core.Tests
{
    using System.Collections.Concurrent;
    using System.Reflection;

    using Gu.Persist.Core;

    using NUnit.Framework;

    public static class RepositoryExt
    {
        private static readonly FieldInfo TrackerCacheField = typeof(DirtyTracker).GetField("cache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

        public static FileCache GetCache(this ISingletonRepository repo)
        {
            if (repo == null)
            {
                return null;
            }

            // ReSharper disable once PossibleNullReferenceException
            var cacheField = repo.GetType().BaseType.GetField("fileCache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            Assert.NotNull(cacheField);
            return (FileCache)cacheField.GetValue(repo);
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
