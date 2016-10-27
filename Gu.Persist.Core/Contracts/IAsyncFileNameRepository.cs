namespace Gu.Persist.Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// A repository that reads and saves async.
    /// </summary>
    public interface IAsyncFileNameRepository : IFileNameRepository
    {
        /// <summary>
        /// <see cref="IRepository.ReadAsync{T}(string)"/>
        /// </summary>
        Task<T> ReadAsync<T>(string fileName);

        /// <summary>
        /// <see cref="IRepository.SaveAsync{T}(T,string)"/>
        /// </summary>
        Task SaveAsync<T>(T item, string fileName);
    }
}