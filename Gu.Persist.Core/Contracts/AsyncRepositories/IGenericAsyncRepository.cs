namespace Gu.Persist.Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// Uses typeof(T).Name.Extension as filename.
    /// </summary>
    public interface IGenericAsyncRepository : IGenericRepository
    {
        /// <summary>
        /// <see cref="IRepository.ReadAsync{T}()"/>
        /// </summary>
        Task<T> ReadAsync<T>();

        /// <summary>
        /// <see cref="IRepository.SaveAsync{T}(T)"/>
        /// </summary>
        Task SaveAsync<T>(T item);
    }
}