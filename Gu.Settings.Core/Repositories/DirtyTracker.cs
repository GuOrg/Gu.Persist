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
        private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private bool _disposed;

        public DirtyTracker(ICloner cloner)
        {
            Ensure.NotNull(cloner, nameof(cloner));
            _cloner = cloner;
        }

        public void Track<T>(FileInfo file, T item)
        {
            Ensure.NotNull(file, nameof(file));
            VerifyDisposed();
            var clone = _cloner.Clone(item);
            _cache.AddOrUpdate(file.FullName, clone, (f, o) => clone);
        }

        public void Rename(FileInfo oldName, FileInfo newName, bool owerWrite)
        {
            Ensure.NotNull(oldName, nameof(oldName));
            Ensure.NotNull(newName, nameof(newName));
            VerifyDisposed();
            _cache.ChangeKey(oldName.FullName, newName.FullName, owerWrite);
        }

        public void ClearCache()
        {
            _cache.Clear();
        }

        public void RemoveFromCache(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            VerifyDisposed();
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
        public bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(comparer, nameof(comparer));
            VerifyDisposed();
            object clone;
            if (_cache.TryGetValue(file.FullName, out clone))
            {
                return !comparer.Equals((T)clone, item);
            }
            return item != null;
        }

        /// <summary>
        /// Make the class sealed when using this. 
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _cache.Clear();
             // Dispose some stuff now
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType()
                        .FullName);
            }
        }
    }
}
