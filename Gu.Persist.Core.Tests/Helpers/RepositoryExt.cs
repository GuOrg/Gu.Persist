namespace Gu.Persist.Core.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    public static class RepositoryExt
    {
        public static FileInfo GetTestFileInfo(this IRepository repository, [CallerMemberName] string name = null) => repository.GetFileInfo(name);

        public static FileInfo GetTestBackupFileInfo(this IRepository repository, [CallerMemberName] string name = null)
        {
            return repository.Settings.BackupSettings is IBackupSettings backupSettings
                ? BackupFile.CreateFor(GetTestFileInfo(repository, name), backupSettings)
                : null;
        }

#pragma warning disable CA1707, SA1313 // Identifiers should not contain underscores
        public static FileInfo GetGenericTestFileInfo<T>(this IRepository repository, T _ = default) => repository.GetFileInfo(typeof(T).Name);
#pragma warning restore CA1707, SA1313 // Identifiers should not contain underscores

#pragma warning disable CA1707, SA1313 // Identifiers should not contain underscores
        public static FileInfo GetGenericTestBackupFileInfo<T>(this IRepository repository, T _ = default)
#pragma warning restore CA1707, SA1313 // Identifiers should not contain underscores
        {
            return repository.Settings.BackupSettings is IBackupSettings backupSettings
                ? BackupFile.CreateFor(GetGenericTestFileInfo<T>(repository), backupSettings)
                : null;
        }

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
