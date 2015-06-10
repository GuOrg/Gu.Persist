namespace Gu.Settings
{
    using System.Threading.Tasks;

    public interface IAutoAsyncRepository
    {
        IRepositorySetting Setting { get; }

        Task<T> ReadAsync<T>();

        Task SaveAsync<T>(T item);
    }
}