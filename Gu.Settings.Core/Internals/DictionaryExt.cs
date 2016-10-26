namespace Gu.Settings.Core.Internals
{
    using System;
    using System.Collections.Concurrent;

    public static class DictionaryExt
    {
        public static void ChangeKey<TKey, TValue>(
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
                    var message = string.Format("Changing key from {0} to {1} failed. Dictionary already has tokey", fromKey,
    tokey);
                    throw new InvalidOperationException(message);
                }
            }

            if (dictionary.TryRemove(fromKey, out value))
            {
                if (dictionary.TryAdd(tokey, value))
                {
                    return;
                }

                var message = string.Format("Could not add {0} to dictionary", fromKey);
                throw new InvalidOperationException(message);
            }
        }
    }
}
