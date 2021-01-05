namespace Gu.Persist.Core
{
    using System;

    /// <summary>
    /// A pooled stream.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public interface IPooledStream : IDisposable
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
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