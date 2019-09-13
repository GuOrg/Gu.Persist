namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// A repository that manages singleton instances.
    /// </summary>
    public interface ISingletonRepository : IRepository
    {
        /// <summary>
        /// Clears the cache.
        ///  </summary>
        /// <remarks>
        /// Calling this means that singletons will no longer be returned by the repository.
        /// </remarks>
        void ClearCache();

        /// <summary>
        /// Removes <paramref name="item"/> from cache.
        /// Next read will read a new instance from disk.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The <see cref="T"/>.</param>
        void RemoveFromCache<T>(T item);

        /// <summary>
        /// Saves <paramref name="item"/> the file specified by <typeparamref name="T"/>. Then removes it from cache.
        /// <seealso cref="IRepository.Save{T}(T)"/>
        /// <seealso cref="ISingletonRepository.RemoveFromCache{T}(T)"/>
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The <see cref="T"/>.</param>
        void SaveAndClose<T>(T item);

        /// <summary>
        /// Saves <paramref name="item"/> the file specified by <paramref name="fileName"/>. Then removes it from cache.
        /// <seealso cref="IRepository.Save{T}(string,T)"/>
        /// <seealso cref="ISingletonRepository.RemoveFromCache{T}(T)"/>
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <param name="item">The item to save.</param>
        void SaveAndClose<T>(string fileName, T item);

        /// <summary>
        /// Saves the file. Then removes it from cache.
        /// <seealso cref="IRepository.Save{T}(FileInfo, T)"/>
        /// <seealso cref="ISingletonRepository.RemoveFromCache{T}(T)"/>
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The <see cref="T"/>.</param>
        void SaveAndClose<T>(FileInfo file, T item);
    }
}