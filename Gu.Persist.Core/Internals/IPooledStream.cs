namespace Gu.Persist.Core
{
    using System;

    public interface IPooledStream : IDisposable
    {
        /// <summary>
        /// See <see cref="System.IO.MemoryStream.Length"/>
        /// </summary>
        long Length { get; }

        /// <summary>
        /// See <see cref="System.IO.MemoryStream.GetBuffer()"/>
        /// </summary>
        byte[] GetBuffer();
    }
}