namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;

    public class DirtyTracker
    {
        private readonly ICloner _cloner;
        private readonly ConcurrentDictionary<FileInfo, object> _cache = new ConcurrentDictionary<FileInfo, object>(FileInfoComparer.Default);
        public DirtyTracker(ICloner cloner)
        {
            _cloner = cloner;
        }

        public void TrackOrUpdate<T>(FileInfo file, T item)
        {
            var clone = _cloner.Clone(item);
            _cache.AddOrUpdate(file, clone, (f, o) => clone);
        }

        public bool IsDirty<T>(FileInfo file, T item)
        {
            return IsDirty(file, item, EqualityComparer<T>.Default);
        }

        public bool IsDirty<T>(FileInfo file, T item, IEqualityComparer<T> comparer)
        {
            object clone;
            if (_cache.TryGetValue(file, out clone))
            {
                return !comparer.Equals((T)clone, item);
            }
            return false;
        }
    }
}
