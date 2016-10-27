namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Concurrent;

    internal static class DictionaryExt
    {
        internal static void ChangeKey<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dictionary,
            TKey fromKey,
            TKey tokey,
            bool owerWrite)
        {
                        TValue value;
            if (dictionary.ContainsKey(tokey))
            {
                if (owerWrite)
                {
                    dictionary.TryRemove(tokey, out value);
                }
                else
                {
                    var message = $"Changing key from {fromKey} to {tokey} failed. Dictionary already has tokey";
                    throw new InvalidOperationException(message);
                }
            }

            if (dictionary.TryRemove(fromKey, out value))
            {
                if (dictionary.TryAdd(tokey, value))
                {
                    return;
                }

                var message = $"Could not add {fromKey} to dictionary";
                throw new InvalidOperationException(message);
            }
        }
    }
}
