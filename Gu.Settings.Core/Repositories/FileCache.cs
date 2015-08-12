namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using Gu.Settings.Core.Internals;

    public sealed class FileCache : IDisposable
    {
        private readonly ConcurrentDictionary<string, WeakReference> _cache = new ConcurrentDictionary<string, WeakReference>(StringComparer.OrdinalIgnoreCase);
        private readonly object _gate = new object();
        private bool _disposed;

        public bool TryGetValue<T>(string fullFileName, out T cached)
        {
            VerifyDisposed();
            WeakReference wr;
            if (_cache.TryGetValue(fullFileName, out wr))
            {
                var target = (T)wr.Target;
                if (target != null)
                {
                    cached = target;
                    return true;
                }
            }
            cached = default(T);
            return false;
        }

        public void Add<T>(string fullName, T value)
        {
            Ensure.NotNullOrEmpty(fullName, nameof(fullName));
            if (value == null)
            {
                return;
            }
            VerifyDisposed();
            lock (_gate)
            {
                _cache.AddOrUpdate(fullName, new WeakReference(value),
                    (key, wr) =>
                        {
                            wr.Target = value;
                            return wr;
                        });
            }
        }

        public bool ContainsKey(string fullName)
        {
            Ensure.NotNullOrEmpty(fullName, nameof(fullName));

            VerifyDisposed();
            WeakReference temp;
            return _cache.TryGetValue(fullName, out temp);
        }

        public void ChangeKey(string @from, string to, bool owerWrite)
        {
            VerifyDisposed();
            lock (_gate)
            {
                _cache.ChangeKey(@from, to, owerWrite);
            }
        }

        public void Clear()
        {
            lock (_gate)
            {
                // not sure about disposing here.
                //foreach (var reference in _cache.Values)
                //{
                //    var disposable = reference.Target as IDisposable;
                //    if (disposable != null)
                //    {
                //        disposable.Dispose();
                //    }
                //}
                _cache.Clear();
            }
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
            Clear();
            // Dispose some stuff now
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public void TryRemove<T>(T item)
        {
            lock (_gate)
            {
                var matches = _cache.Where(kvp => kvp.Value != null && ReferenceEquals(kvp.Value.Target, item))
                    .Select(x => x.Key)
                    .ToArray();
                foreach (var key in matches)
                {
                    WeakReference temp;
                    _cache.TryRemove(key, out temp);
                }
            }
        }
    }
}
