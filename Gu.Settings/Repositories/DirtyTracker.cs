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
            _cloner = cloner;
        }

        public virtual void TrackOrUpdate<T>(FileInfo file, T item)
        {
            var clone = _cloner.Clone(item);
            _cache.AddOrUpdate(file.FullName, clone, (f, o) => clone);
        }

        public void Rename(FileInfo oldName, FileInfo newName, bool owerWrite)
        {
            Ensure.NotNull(oldName, "oldName");
            Ensure.NotNull(newName, "newName");
            _cache.ChangeKey(oldName.FullName, newName.FullName, owerWrite);
        }

        public virtual bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            object clone;
            if (_cache.TryGetValue(file.FullName, out clone))
            {
                return !comparer.Equals((T)clone, item);
            }
            return item != null;
        }
    }
}
