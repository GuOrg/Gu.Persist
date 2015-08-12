namespace Gu.Settings
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository for async reads and writes.
    /// </summary>
    public interface IAsyncRepository
    {
        /// <see cref="IRepository.Delete{T}(bool)"/>
        void Delete<T>(bool deleteBackups);

        /// <see cref="IRepository.Delete(string, bool)"/>
        void Delete(string fileName, bool deleteBackups);

        /// <see cref="IRepository.Delete(FileInfo, bool)"/>
        void Delete(FileInfo file, bool deleteBackups);

        /// <see cref="IRepository.GetFileInfo{T}()"/>
        FileInfo GetFileInfo<T>();

        /// <see cref="IRepository.GetFileInfo(string)"/>
        FileInfo GetFileInfo(string fileName);

        /// <see cref="IRepository.Read{T}(string)"/>
        Task<T> ReadAsync<T>(string fileName);

        /// <see cref="IRepository.Read{T}(FileInfo)"/>
        Task<T> ReadAsync<T>(FileInfo file);

        /// <see cref="IRepository.Save{T}(T, string)"/>
        Task SaveAsync<T>(T item, string fileName);

        /// <see cref="IRepository.Save{T}(T, FileInfo)"/>
        Task SaveAsync<T>(T item, FileInfo file);

        /// <see cref="IRepository.Save{T}(T, FileInfo, FileInfo)"/>
        Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile);
    }
}