namespace Gu.Settings
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IAsyncRepository
    {
        RepositorySetting Setting { get; }

        Task<T> ReadAsync<T>(string fileName);
        
        Task<T> ReadAsync<T>(FileInfo file);

        Task SaveAsync<T>(T item, string fileName);

        Task SaveAsync<T>(T item, FileInfo file);

        /// <summary>
        /// The file is saved to tempFile, if successful tempFile is renamed to file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        /// <param name="tempFile"></param>
        /// <returns></returns>
        Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile);
    }
}