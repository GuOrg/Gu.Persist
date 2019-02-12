namespace Gu.Persist.Core
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Use this for reading the raw streams.
    /// When using streams no caching nor dirtytracking is performed.
    /// </summary>
    public interface IFileInfoAsyncStreamRepository
    {
        /// <summary>
        /// Reads the file <paramref name="file"/> and returns the contents in a memorystream.
        /// </summary>
        /// <remarks>
        /// This method reads the entire file into memory so it will be memory consuming for large files.
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        Task<Stream> ReadAsync(FileInfo file);

        /// <summary>
        /// Save to <paramref name="stream"/> <paramref name="file"/>.
        /// </summary>
        /// <remarks>
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        Task SaveAsync(FileInfo file, Stream stream);

        /// <summary>
        /// Save to <paramref name="stream"/> <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The file to save to.</param>
        /// <param name="tempFile">The temporary file to use when saving.</param>
        /// <param name="stream">The contents.</param>
        /// <remarks>
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        Task SaveAsync(FileInfo file, FileInfo tempFile, Stream stream);
    }
}