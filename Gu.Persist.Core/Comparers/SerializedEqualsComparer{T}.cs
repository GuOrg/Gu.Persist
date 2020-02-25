namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A base class for comparers using serialization.
    /// </summary>
    /// <typeparam name="T">The type to read from the file.</typeparam>
    public abstract class SerializedEqualsComparer<T> : EqualityComparer<T>
    {
        /// <inheritdoc />
        public override bool Equals(T x, T y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
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

        /// <inheritdoc />
        public override int GetHashCode(T obj)
        {
            if (obj is null)
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
        /// Serialize <paramref name="item"/> and return the bytes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A <see cref="IPooledStream"/>.</returns>
        protected abstract IPooledStream GetStream(T item);
    }
}
