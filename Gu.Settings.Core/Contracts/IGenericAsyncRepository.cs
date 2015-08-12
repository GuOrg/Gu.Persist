namespace Gu.Settings.Core
{
    using System.Threading.Tasks;

    public interface IGenericAsyncRepository : IGenericRepository
    {
        /// <see cref="IRepository.Read{T}()"/>
        Task<T> ReadAsync<T>();

        /// <see cref="IRepository.Save{T}(T)"/>
        Task SaveAsync<T>(T item);
    }
}