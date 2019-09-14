namespace Gu.Persist.Core.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Gu.Persist.Core;

    using NUnit.Framework;

    public static class RepositoryExt
    {
        public static FileInfo GetTestFileInfo(this IRepository repository, [CallerMemberName] string name = null) => repository.GetFileInfo(name);

        public static FileInfo GetGenericTestFileInfo<T>(this IRepository repository, T _ = default) => repository.GetFileInfo(typeof(T).Name);

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

        public static void ClearCache(this IRepository repository)
        {
            var cache = (repository as ISingletonRepository)?.GetCache();
            cache?.Clear(); // nUnit does some optimization keeping this alive
        }
    }
}
