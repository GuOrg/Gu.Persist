namespace Gu.Settings
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IAsyncRepository
    {
        void Delete<T>(bool deleteBackups);

        void Delete(string fileName, bool deleteBackups);

        void Delete(FileInfo file, bool deleteBackups);

        /// <summary>
        /// Gets the fileinfo for that is used for the type
        /// </summary
        /// <param name="fileName"></param>
        /// <returns></returns>
        FileInfo GetFileInfo<T>();

        /// <summary>
        /// Gets the fileinfo for that is used for the given filename
        /// </summary
        /// <param name="fileName"></param>
        /// <returns></returns>
        FileInfo GetFileInfo(string fileName);

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