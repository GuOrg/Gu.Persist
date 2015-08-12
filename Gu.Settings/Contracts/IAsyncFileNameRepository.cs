namespace Gu.Settings
{
    using System.Threading.Tasks;

    /// <summary>
    /// A repsository that reads and saves 
    /// </summary>
    public interface IAsyncFileNameRepository : IFileNameRepository
    {
        /// <see cref="IRepository.Read{T}(string)"/>
        Task<T> ReadAsync<T>(string fileName);

        /// <see cref="IRepository.Save{T}(T,string)"/>
        Task SaveAsync<T>(T item, string fileName);
    }
}