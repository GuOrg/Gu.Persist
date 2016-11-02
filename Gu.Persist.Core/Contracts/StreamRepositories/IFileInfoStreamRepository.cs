namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// Use this for reading the raw streams.
    /// Not that caching and dirty tracking does not work for streams.
    /// </summary>
    public interface IFileInfoStreamRepository
    {
        /// <summary>
        /// Reads the file <paramref name="file"/> and returns the contents in a memorystream
        /// </summary>
        Stream Read(FileInfo file);

        /// <summary>
        /// Saves the stream and creates backups.
        /// </summary>
        void Save(FileInfo file, Stream stream);

        /// <summary>
        /// Saves <paramref name="stream"/> to <paramref name="file"/>
        /// <seealso cref="IRepository.Save{T}(FileInfo, FileInfo, T)"/>
        /// </summary>
        void Save(FileInfo file, FileInfo tempFile, Stream stream);
    }
}