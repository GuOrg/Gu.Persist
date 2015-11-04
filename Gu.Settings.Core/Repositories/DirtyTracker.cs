namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.Core.Internals;

    public sealed class DirtyTracker : IDirtyTracker
    {
        private readonly ICloner _cloner;
        private readonly ConcurrentDictionary<string, object> _clones = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private readonly object _gate = new object();

        public DirtyTracker(ICloner cloner)
        {
            Ensure.NotNull(cloner, nameof(cloner));
            _cloner = cloner;
        }

        public void Track<T>(string fullFileName, T item)
        {
            Ensure.NotNull(fullFileName, nameof(fullFileName));
            var clone = _cloner.Clone(item);
            lock (_gate)
            {
                _clones.AddOrUpdate(fullFileName, clone, (f, o) => clone);
            }
        }

        public void Rename(string oldName, string newName, bool owerWrite)
        {
            Ensure.NotNull(oldName, nameof(oldName));
            Ensure.NotNull(newName, nameof(newName));
            lock (_gate)
            {
                _clones.ChangeKey(oldName, newName, owerWrite);
            }
        }

        public void ClearCache()
        {
            lock (_gate)
            {
                _clones.Clear();
            }
        }

        public void RemoveFromCache(string fullFileName)
        {
            Ensure.NotNull(fullFileName, nameof(fullFileName));
            lock (_gate)
            {
                object temp;
                _clones.TryRemove(fullFileName, out temp);
            }
        }

        /// <summary>
        /// Only checks the cache, does not read from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fullFileName"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool IsDirty<T>(T item, string fullFileName, IEqualityComparer<T> comparer)
        {
            Ensure.NotNull(fullFileName, nameof(fullFileName));
            Ensure.NotNull(comparer, nameof(comparer));
            object clone;
            lock (_gate)
            {
                _clones.TryGetValue(fullFileName, out clone);
            }
            return !comparer.Equals((T)clone, item);
        }
    }
}
