namespace Gu.Settings
{
    using System.Threading.Tasks;

    public interface IGenericAsyncRepository : IGenericRepository
    {
        Task<T> ReadAsync<T>();

        Task SaveAsync<T>(T item);
    }
}