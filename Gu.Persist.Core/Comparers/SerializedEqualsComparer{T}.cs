namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            using (var xStream = this.GetStream(x))
            using (var yStream = this.GetStream(y))
            {
                if (xStream.Length != yStream.Length)
                {
                    return false;
                }

                var xBytes = xStream.GetBuffer();
                var yBytes = yStream.GetBuffer();
                for (var i = 0; i < xStream.Length; i++)
                {
                    if (xBytes[i] != yBytes[i])
                    {
                        return false;
                    }
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

            using (var stream = this.GetStream(obj))
            {
                var bytes = stream.GetBuffer();
                unchecked
                {
                    var hash = 17;
                    for (var i = 0; i < stream.Length; i++)
                    {
                        hash = (hash * 31) + bytes[i];
                    }

                    return hash;
                }
            }
        }

        /// <summary>
        /// Serialize <paramref name="item"/> and return the bytes
        /// </summary>
        protected abstract IPooledStream GetStream(T item);
    }
}
