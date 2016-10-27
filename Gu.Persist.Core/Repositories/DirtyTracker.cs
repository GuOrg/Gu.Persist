namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public sealed class DirtyTracker : IDirtyTracker
    {
        private readonly ICloner cloner;
        private readonly ConcurrentDictionary<string, object> clones = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private readonly object gate = new object();

        public DirtyTracker(ICloner cloner)
        {
            Ensure.NotNull(cloner, nameof(cloner));
            this.cloner = cloner;
        }

        public void Track<T>(string fullFileName, T item)
        {
            Ensure.NotNull(fullFileName, nameof(fullFileName));
            var clone = this.cloner.Clone(item);
            lock (this.gate)
            {
                this.clones.AddOrUpdate(fullFileName, clone, (f, o) => clone);
            }
        }

        public void Rename(string oldName, string newName, bool owerWrite)
        {
            Ensure.NotNull(oldName, nameof(oldName));
            Ensure.NotNull(newName, nameof(newName));
            lock (this.gate)
            {
                this.clones.ChangeKey(oldName, newName, owerWrite);
            }
        }

        public void ClearCache()
        {
            lock (this.gate)
            {
                this.clones.Clear();
            }
        }

        public void RemoveFromCache(string fullFileName)
        {
            Ensure.NotNull(fullFileName, nameof(fullFileName));
            lock (this.gate)
            {
                object temp;
                this.clones.TryRemove(fullFileName, out temp);
            }
        }

        /// <summary>
        /// Only checks the cache, does not read from file.
        /// </summary>
        public bool IsDirty<T>(T item, string fullFileName, IEqualityComparer<T> comparer)
        {
            Ensure.NotNull(fullFileName, nameof(fullFileName));
            Ensure.NotNull(comparer, nameof(comparer));
            object clone;
            lock (this.gate)
            {
                this.clones.TryGetValue(fullFileName, out clone);
            }

            return !comparer.Equals((T)clone, item);
        }
    }
}
