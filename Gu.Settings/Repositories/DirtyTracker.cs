namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using Internals;

    public class DirtyTracker : IDirtyTracker
    {
        private readonly ICloner _cloner;
        private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public DirtyTracker(ICloner cloner)
        {
            Ensure.NotNull(cloner, "cloner");
            _cloner = cloner;
        }

        public virtual void TrackOrUpdate<T>(FileInfo file, T item)
        {
            Ensure.NotNull(file, "file");
            var clone = _cloner.Clone(item);
            _cache.AddOrUpdate(file.FullName, clone, (f, o) => clone);
        }

        public void Rename(FileInfo oldName, FileInfo newName, bool owerWrite)
        {
            Ensure.NotNull(oldName, "oldName");
            Ensure.NotNull(newName, "newName");
            _cache.ChangeKey(oldName.FullName, newName.FullName, owerWrite);
        }

        public void ClearCache()
        {
            _cache.Clear();
        }

        public void RemoveFromCache(FileInfo file)
        {
            Ensure.NotNull(file, "file");
            object temp;
            _cache.TryRemove(file.FullName, out temp);
        }

        /// <summary>
        /// Only checks the cache, does not read from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public virtual bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            Ensure.NotNull(file, "file");
            Ensure.NotNull(comparer, "comparer");
            object clone;
            if (_cache.TryGetValue(file.FullName, out clone))
            {
                return !comparer.Equals((T)clone, item);
            }
            return item != null;
        }
    }
}
