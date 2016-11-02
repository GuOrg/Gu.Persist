namespace Gu.Persist.Core
{
    using System.IO;

    public interface IFileInfoRepository
    {
        /// <summary>
        /// Reads the contents of <paramref name="file"/> and deserializes it to an instance of <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        T Read<T>(FileInfo file);

        /// <summary>
        /// Saves <see paramref="item"/> to <paramref name="file"/>
        /// <seealso cref="IRepository.Save{T}(FileInfo, FileInfo, T)"/>
        /// </summary>
        void Save<T>(FileInfo file, T item);
    }
}