namespace Gu.Persist.Core
{
    using System;

    /// <summary>
    /// A pooled stream.
    /// </summary>
    public interface IPooledStream : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="System.IO.MemoryStream.Length"/>.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// See <see cref="System.IO.MemoryStream.GetBuffer()"/>.
        /// </summary>
        /// <returns>The array of bytes.</returns>
        byte[] GetBuffer();
    }
}