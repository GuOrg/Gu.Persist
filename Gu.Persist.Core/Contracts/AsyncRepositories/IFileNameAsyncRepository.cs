namespace Gu.Persist.Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// A repository that reads and saves async.
    /// </summary>
    public interface IFileNameAsyncRepository
    {
        /// <summary>
        /// <see cref="IRepository.ReadAsync{T}(string)"/>
        /// </summary>
        Task<T> ReadAsync<T>(string fileName);

        /// <summary>
        /// <see cref="IRepository.SaveAsync{T}(string,T)"/>
        /// </summary>
        Task SaveAsync<T>(string fileName, T item);
    }
}