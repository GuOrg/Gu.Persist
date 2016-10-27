namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A base class for comparers using serialization
    /// </summary>
    public abstract class SerializedEqualsComparer<T> : EqualityComparer<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly byte[] EmptyBytes = new byte[0];

        /// <summary>
        /// Serializes <paramref name="x"/> and <paramref name="y"/> and compares the bytes.
        /// </summary>
        public override bool Equals(T x, T y)
        {
            var xBytes = this.GetBytesInner(x);
            var yBytes = this.GetBytesInner(y);
            if (xBytes.Length != yBytes.Length)
            {
                return false;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery, clearer with for imo
            for (var i = 0; i < xBytes.Length; i++)
            {
                if (xBytes[i] != yBytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Serializes <paramref name="obj"/> and calculates hashcode from the bytes.
        /// http://stackoverflow.com/a/7244729/1069200
        /// </summary>
        public override int GetHashCode(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var bytes = this.GetBytesInner(obj);
            unchecked
            {
                var hash = 17;
                for (var i = 0; i < bytes.Length; i++)
                {
                    hash = (hash * 31) + bytes[i];
                }

                return hash;
            }
        }

        /// <summary>
        /// Serialize <paramref name="item"/> and return the bytes
        /// </summary>
        protected abstract byte[] GetBytes(T item);

        private byte[] GetBytesInner(T item)
        {
            if (item == null)
            {
                return EmptyBytes;
            }

            return this.GetBytes(item);
        }
    }
}
