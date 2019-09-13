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
            this.cloner = cloner ?? throw new ArgumentNullException(nameof(cloner));
        }

        public void Track<T>(string fullFileName, T item)
        {
            Ensure.NotNullOrEmpty(fullFileName, nameof(fullFileName));
            var clone = item == null
                            ? (object)null
                            : this.cloner.Clone(item);
            lock (this.gate)
            {
                this.clones.AddOrUpdate(fullFileName, clone, (f, o) => clone);
            }
        }

        public void Rename(string oldName, string newName, bool overWrite)
        {
            Ensure.NotNullOrEmpty(oldName, nameof(oldName));
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            lock (this.gate)
            {
                this.clones.ChangeKey(oldName, newName, overWrite);
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
            if (fullFileName is null)
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            lock (this.gate)
            {
                this.clones.TryRemove(fullFileName, out _);
            }
        }

        /// <summary>
        /// Only checks the cache, does not read from file.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The <see cref="T"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        public bool IsDirty<T>(string fullFileName, T item, IEqualityComparer<T> comparer)
        {
            Ensure.NotNullOrEmpty(fullFileName, nameof(fullFileName));
            comparer = comparer ?? EqualityComparer<T>.Default;
            object clone;
            lock (this.gate)
            {
                this.clones.TryGetValue(fullFileName, out clone);
            }

            return !comparer.Equals((T)clone, item);
        }
    }
}
