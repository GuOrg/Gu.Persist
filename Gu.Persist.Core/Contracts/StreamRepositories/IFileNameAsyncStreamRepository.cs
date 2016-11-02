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
        /// <see cref="IRepository.Read{T}(string)"/>
        /// </summary>
        Task<Stream> ReadAsync(string fileName);

        /// <summary>
        /// <see cref="IRepository.Save{T}(string, T)"/>
        /// </summary>
        Task SaveAsync(string fileName, Stream stream);
    }
}