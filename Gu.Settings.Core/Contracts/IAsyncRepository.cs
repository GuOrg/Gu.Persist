namespace Gu.Settings.Core
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository for async reads and writes.
    /// </summary>
    public interface IAsyncRepository
    {
        /// <summary>
        /// <see cref="IRepository.Delete{T}(bool)"/>
        /// </summary>
        void Delete<T>(bool deleteBackups);

        /// <summary>
        /// <see cref="IRepository.Delete(string, bool)"/>
        /// </summary>
        void Delete(string fileName, bool deleteBackups);

        /// <summary>
        /// <see cref="IRepository.Delete(FileInfo, bool)"/>
        /// </summary>
        void Delete(FileInfo file, bool deleteBackups);

        /// <summary>
        /// <see cref="IRepository.GetFileInfo{T}()"/>
        /// </summary>
        FileInfo GetFileInfo<T>();

        /// <summary>
        /// <see cref="IRepository.GetFileInfo(string)"/>
        /// </summary>
        FileInfo GetFileInfo(string fileName);

        /// <summary>
        /// <see cref="IRepository.Read{T}(string)"/>
        /// </summary>
        Task<T> ReadAsync<T>(string fileName);

        /// <summary>
        /// <see cref="IRepository.Read{T}(FileInfo)"/>
        /// </summary>
        Task<T> ReadAsync<T>(FileInfo file);

        /// <summary>
        /// <see cref="IRepository.Save{T}(T, string)"/>
        /// </summary>
        Task SaveAsync<T>(T item, string fileName);

        /// <summary>
        /// <see cref="IRepository.Save{T}(T, FileInfo)"/>
        /// </summary>
        Task SaveAsync<T>(T item, FileInfo file);

        /// <summary>
        /// <see cref="IRepository.Save{T}(T, FileInfo, FileInfo)"/>
        /// </summary>
        Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile);
    }
}