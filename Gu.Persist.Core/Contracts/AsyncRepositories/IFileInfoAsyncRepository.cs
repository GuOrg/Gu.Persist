namespace Gu.Persist.Core
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository that reads and saves async.
    /// </summary>
    public interface IFileInfoAsyncRepository
    {
        /// <summary>
        /// <see cref="IRepository.ReadAsync{T}(FileInfo)"/>
        /// </summary>
        Task<T> ReadAsync<T>(FileInfo file);

        /// <summary>
        /// <see cref="IRepository.SaveAsync{T}(FileInfo,T)"/>
        /// </summary>
        Task SaveAsync<T>(FileInfo fileName, T item);
    }
}