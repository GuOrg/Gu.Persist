namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// Use this for reading the raw streams.
    /// When using streams no caching nor dirtytracking is performed.
    /// </summary>
    public interface IFileInfoStreamRepository
    {
        /// <summary>
        /// Read from <paramref name="file"/>.
        /// </summary>
        /// <remarks>
        /// This method reads the entire file into memory so it will be memory consuming for large files.
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        /// <returns>A stream with the contents of the file.</returns>
        Stream Read(FileInfo file);

        /// <summary>
        /// Save to <paramref name="stream"/> <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The file to save to.</param>
        /// <param name="stream">The contents</param>
        /// <remarks>
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        void Save(FileInfo file, Stream stream);

        /// <summary>
        /// Save to <paramref name="stream"/> <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The file to save to.</param>
        /// <param name="tempFile">The temporary file to use when saving.</param>
        /// <param name="stream">The contents</param>
        /// <remarks>
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        void Save(FileInfo file, FileInfo tempFile, Stream stream);
    }
}