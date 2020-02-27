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
            where TKey : notnull
        {
            if (dictionary.TryRemove(fromKey, out var value))
            {
                if (overWrite)
                {
                    dictionary[toKey] = value;
                }
                else
                {
                    if (dictionary.TryAdd(toKey, value))
                    {
                        return;
                    }

                    throw new InvalidOperationException($"Changing key from {fromKey} to {toKey} failed. Dictionary already has toKey");
                }
            }
            else
            {
                if (overWrite)
                {
                    dictionary[toKey] = value;
                }
                else
                {
                    if (dictionary.TryAdd(toKey, value))
                    {
                        return;
                    }

                    throw new InvalidOperationException($"Changing key from {fromKey} to {toKey} failed. Dictionary already has toKey");
                }
            }
        }
    }
}
