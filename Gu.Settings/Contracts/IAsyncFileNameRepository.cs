namespace Gu.Settings
{
    using System.Threading.Tasks;

    public interface IAsyncFileNameRepository : IFileNameRepository
    {
        /// <summary>
        /// Reads from file or cache if caching. 
        /// If caching every read will get the same singleton instance.
        /// Adds the instance to cache if caching.
        /// Starts tracking the if tracking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <returns></returns>
        Task<T> ReadAsync<T>(string fileName);

        /// <summary>
        /// Saves the item to file.
        /// Adds it to cache if caching and first time it is read/saved
        /// Starts tracking if tracking changes.
        /// </summary>
        /// <typeparam name="T">Not used for anything, maybe useful for constraining?</typeparam>
        /// <param name="item"></param>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        Task SaveAsync<T>(T item, string fileName);
    }
}