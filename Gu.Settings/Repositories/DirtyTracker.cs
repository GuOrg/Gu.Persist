namespace Gu.Settings
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;

    public class DirtyTracker : IDirtyTracker
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

        public bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            object clone;
            if (_cache.TryGetValue(file, out clone))
            {
                return !comparer.Equals((T)clone, item);
            }
            return item != null;
        }
    }
}
