namespace Gu.Persist.Core
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Use this for reading the raw streams.
    /// Not that caching and dirty tracking does not work for streams.
    /// </summary>
    public interface IFileInfoAsyncStreamRepository
    {
        /// <summary>
        ///  <see cref="IFileInfoStreamRepository.Read(FileInfo)"/>
        /// </summary>
        Task<Stream> ReadAsync(FileInfo file);

        /// <summary>
        /// <see cref="IFileInfoStreamRepository.Save(FileInfo,Stream)"/>
        /// </summary>
        Task SaveAsync(FileInfo file, Stream stream);

        /// <summary>
        /// <see cref="IFileInfoStreamRepository.Save(FileInfo, FileInfo, Stream)"/>
        /// </summary>
        Task SaveAsync(FileInfo file, FileInfo tempFile, Stream stream);
    }
}