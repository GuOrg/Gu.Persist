namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// For caching an instance by corresponding file name.
    /// </summary>
    public sealed class FileCache
    {
        private readonly ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private readonly object gate = new object();

        /// <summary>
        /// Get the cached instance for a <paramref name="fullFileName"/> if it exists.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="cached"/>.</typeparam>
        /// <param name="fullFileName">The file name.</param>
        /// <param name="cached">The cahced instance.</param>
        /// <returns>True if a cached instance for a <paramref name="fullFileName"/> exists.</returns>
        public bool TryGetValue<T>(string fullFileName, [MaybeNullWhen(false)] out T cached)
        {
            lock (this.gate)
            {
                if (this.cache.TryGetValue(fullFileName, out var value))
                {
                    cached = (T)value;
                    return true;
                }
            }

            cached = default!;
            return false;
        }

        /// <summary>
        /// Add <paramref name="value"/> to the cache.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
        /// <param name="fullFileName">The file name.</param>
        /// <param name="value">The value.</param>
        public void Add<T>(string fullFileName, T value)
        {
            if (string.IsNullOrEmpty(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            if (value is null)
            {
                return;
            }

            lock (this.gate)
            {
                _ = this.cache.AddOrUpdate(fullFileName, value, (_, __) => value);
            }
        }

        /// <summary>
        /// Check if a value is cached for the file name.
        /// </summary>
        /// <param name="fullFileName">The file name.</param>
        /// <returns>True if a cached value was found.</returns>
        public bool ContainsKey(string fullFileName)
        {
            if (string.IsNullOrEmpty(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            lock (this.gate)
            {
                return this.cache.TryGetValue(fullFileName, out _);
            }
        }

        /// <summary>
        /// Change the key for a cached instance.
        /// </summary>
        /// <param name="from">The old key.</param>
        /// <param name="to">The new key.</param>
        /// <param name="overWrite">Replace if exists.</param>
        public void ChangeKey(string @from, string to, bool overWrite)
        {
            lock (this.gate)
            {
                this.cache.ChangeKey(@from, to, overWrite);
            }
        }

        /// <summary>
        /// Clear the cache.
        /// </summary>
        public void Clear()
        {
            lock (this.gate)
            {
                this.cache.Clear();
            }
        }

        /// <summary>
        /// Remove <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The item.</param>
        public void TryRemove<T>(T item)
        {
            lock (this.gate)
            {
                var matches = this.cache.Where(kvp => kvp.Value != null && ReferenceEquals(kvp.Value, item))
                                    .Select(x => x.Key)
                                    .ToArray();
                foreach (var key in matches)
                {
                    this.cache.TryRemove(key, out var _);
                }
            }
        }
    }
}
