namespace Gu.Settings.Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// A repository that reads and saves async.
    /// </summary>
    public interface IAsyncFileNameRepository : IFileNameRepository
    {
        /// <see cref="IRepository.ReadAsync{T}(string)"/>
        Task<T> ReadAsync<T>(string fileName);

        /// <see cref="IRepository.SaveAsync{T}(T,string)"/>
        Task SaveAsync<T>(T item, string fileName);
    }
}