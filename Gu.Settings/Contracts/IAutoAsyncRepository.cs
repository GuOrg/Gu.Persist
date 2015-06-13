namespace Gu.Settings
{
    using System.Threading.Tasks;

    public interface IAutoAsyncRepository : IAutoRepository
    {
        Task<T> ReadAsync<T>();

        Task SaveAsync<T>(T item);
    }
}