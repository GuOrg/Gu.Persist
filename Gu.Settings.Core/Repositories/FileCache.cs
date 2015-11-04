namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using Gu.Settings.Core.Internals;

    public sealed class FileCache
    {
        private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private readonly object _gate = new object();

        public bool TryGetValue<T>(string fullFileName, out T cached)
        {
            lock (_gate)
            {
                object value;
                if (_cache.TryGetValue(fullFileName, out value))
                {
                    cached = (T)value;
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

            lock (_gate)
            {
                _cache.AddOrUpdate(fullName, value, (_, __) => value);
            }
        }

        public bool ContainsKey(string fullName)
        {
            Ensure.NotNullOrEmpty(fullName, nameof(fullName));

            lock (_gate)
            {
                object temp;
                return _cache.TryGetValue(fullName, out temp);
            }
        }

        public void ChangeKey(string @from, string to, bool owerWrite)
        {
            lock (_gate)
            {
                _cache.ChangeKey(@from, to, owerWrite);
            }
        }

        public void Clear()
        {
            lock (_gate)
            {
                _cache.Clear();
            }
        }

        public void TryRemove<T>(T item)
        {
            lock (_gate)
            {
                var matches = _cache.Where(kvp => kvp.Value != null && ReferenceEquals(kvp.Value, item))
                                    .Select(x => x.Key)
                                    .ToArray();
                foreach (var key in matches)
                {
                    object temp;
                    _cache.TryRemove(key, out temp);
                }
            }
        }
    }
}
