namespace Gu.Settings
{
    using System.Collections.Generic;

    public abstract class SerializedEqualsComparer<T> : IEqualityComparer<T>
    {
        private static readonly byte[] EmptyBytes = new byte[0];

        public bool Equals(T x, T y)
        {
            var xBytes = GetBytesInner(x);
            var yBytes = GetBytesInner(y);
            if (xBytes.Length != yBytes.Length)
            {
                return false;
            }
            // ReSharper disable once LoopCanBeConvertedToQuery, clearer with for imo
            for (int i = 0; i < xBytes.Length; i++)
            {
                if (xBytes[i] != yBytes[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// http://stackoverflow.com/a/7244729/1069200
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            var bytes = GetBytesInner(obj);
            unchecked
            {
                int hash = 17;
                // ReSharper disable once ForCanBeConvertedToForeach, for for perf here. Still going to be slow.
                for (int i = 0; i < bytes.Length; i++)
                {
                    hash = hash * 31 + bytes[i];
                }
                return hash;
            }
        }

        protected abstract byte[] GetBytes(T item);

        private byte[] GetBytesInner(T item)
        {
            if (item == null)
            {
                return EmptyBytes;
            }
            return GetBytes(item);
        }
    }
}
