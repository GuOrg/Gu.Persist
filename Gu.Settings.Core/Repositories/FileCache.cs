namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using Gu.Settings.Core.Internals;

    public sealed class FileCache
    {
        private readonly ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private readonly object gate = new object();

        public bool TryGetValue<T>(string fullFileName, out T cached)
        {
            lock (this.gate)
            {
                object value;
                if (this.cache.TryGetValue(fullFileName, out value))
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

            lock (this.gate)
            {
                this.cache.AddOrUpdate(fullName, value, (_, __) => value);
            }
        }

        public bool ContainsKey(string fullName)
        {
            Ensure.NotNullOrEmpty(fullName, nameof(fullName));

            lock (this.gate)
            {
                object temp;
                return this.cache.TryGetValue(fullName, out temp);
            }
        }

        public void ChangeKey(string @from, string to, bool owerWrite)
        {
            lock (this.gate)
            {
                this.cache.ChangeKey(@from, to, owerWrite);
            }
        }

        public void Clear()
        {
            lock (this.gate)
            {
                this.cache.Clear();
            }
        }

        public void TryRemove<T>(T item)
        {
            lock (this.gate)
            {
                var matches = this.cache.Where(kvp => kvp.Value != null && ReferenceEquals(kvp.Value, item))
                                    .Select(x => x.Key)
                                    .ToArray();
                foreach (var key in matches)
                {
                    object temp;
                    this.cache.TryRemove(key, out temp);
                }
            }
        }
    }
}
