namespace Gu.Persist.Core
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Use this for reading the raw streams.
    /// Not that caching and dirty tracking does not work for streams.
    /// </summary>
    public interface IGenericAsyncStreamRepository
    {
        /// <summary>
        /// <see cref="IGenericAsyncRepository.ReadAsync{T}"/>
        /// </summary>
        Task<Stream> ReadAsync<T>();

        /// <summary>
        /// <see cref="IGenericAsyncRepository.SaveAsync{T}(T)"/>
        /// </summary>
        Task SaveAsync<T>(Stream stream);
    }
}