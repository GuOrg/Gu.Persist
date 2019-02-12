namespace Gu.Persist.Core
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Use this for reading the raw streams.
    /// Not that caching and dirty tracking does not work for streams.
    /// </summary>
    public interface IFileNameAsyncStreamRepository
    {
        /// <summary>
        /// Read from <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <remarks>
        /// This method reads the entire file into memory so it will be memory consuming for large files.
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        /// <returns>A stream with the contents of the file.</returns>
        Task<Stream> ReadAsync(string fileName);

        /// <summary>
        /// Save to <paramref name="stream"/> <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <param name="stream">The contents.</param>
        /// <remarks>
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        Task SaveAsync(string fileName, Stream stream);
    }
}