namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Concurrent;

    internal static class DictionaryExt
    {
        internal static void ChangeKey<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dictionary,
            TKey fromKey,
            TKey toKey,
            bool overWrite)
        {
            TValue value;
            if (dictionary.ContainsKey(toKey))
            {
                if (overWrite)
                {
                    dictionary.TryRemove(toKey, out value);
                }
                else
                {
                    var message = $"Changing key from {fromKey} to {toKey} failed. Dictionary already has toKey";
                    throw new InvalidOperationException(message);
                }
            }

            if (dictionary.TryRemove(fromKey, out value))
            {
                if (dictionary.TryAdd(toKey, value))
                {
                    return;
                }

                var message = $"Could not add {fromKey} to dictionary";
                throw new InvalidOperationException(message);
            }
        }
    }
}
