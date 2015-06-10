namespace Gu.Settings
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IAsyncRepository
    {
        IRepositorySetting Setting { get; }

        Task<T> ReadAsync<T>(string fileName);
        
        Task<T> ReadAsync<T>(FileInfo file);

        Task SaveAsync<T>(T item, string fileName);

        Task SaveAsync<T>(T item, FileInfo file);

        Task SaveAsync<T>(T item, IFileInfos fileInfos);
    }
}