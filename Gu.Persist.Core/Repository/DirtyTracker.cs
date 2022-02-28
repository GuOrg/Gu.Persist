namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Tracks if files have c hanged since last save.
    /// </summary>
    public sealed class DirtyTracker : IDirtyTracker
    {
        private readonly ICloner cloner;
        private readonly ConcurrentDictionary<string, object?> clones = new(StringComparer.OrdinalIgnoreCase);
        private readonly object gate = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DirtyTracker"/> class.
        /// </summary>
        /// <param name="cloner">The <see cref="ICloner"/>.</param>
        public DirtyTracker(ICloner cloner)
        {
            this.cloner = cloner ?? throw new ArgumentNullException(nameof(cloner));
        }

        /// <inheritdoc/>
        public void Track<T>(string fullFileName, T item)
        {
            if (string.IsNullOrEmpty(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            var clone = item is null
                            ? (object?)null
                            : this.cloner.Clone(item);
            lock (this.gate)
            {
                _ = this.clones.AddOrUpdate(fullFileName, clone, (f, o) => clone);
            }
        }

        /// <inheritdoc/>
        public void Rename(string oldName, string newName, bool overWrite)
        {
            if (string.IsNullOrEmpty(oldName))
            {
                throw new ArgumentNullException(nameof(oldName));
            }

            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }

            lock (this.gate)
            {
                this.clones.ChangeKey(oldName, newName, overWrite);
            }
        }

        /// <inheritdoc/>
        public void ClearCache()
        {
            lock (this.gate)
            {
                this.clones.Clear();
            }
        }

        /// <inheritdoc/>
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
        /// <param name="fullFileName">The file name.</param>
        /// <param name="item">The item.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        /// <returns>True if <paramref name="item"/> has changed since last save.</returns>
        public bool IsDirty<T>(string fullFileName, T item, IEqualityComparer<T> comparer)
        {
            if (string.IsNullOrEmpty(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            comparer ??= EqualityComparer<T>.Default;
            object? clone;
            lock (this.gate)
            {
                _ = this.clones.TryGetValue(fullFileName, out clone);
            }

            return !comparer.Equals((T)clone!, item);
        }
    }
}
