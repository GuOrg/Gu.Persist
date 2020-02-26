namespace Gu.Persist.Core
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository that determines filenames from type parameters.
    /// When using this it is important that T is the same when reading and saving.
    /// </summary>
    public interface IGenericAsyncRepository : IGenericRepository
    {
        /// <summary>
        /// Reads from file for <typeparamref name="T"/> and deserializes the contents to an instance of <typeparamref name="T"/>
        /// The filename is typeof(T).Name and the extension specified in settings.
        /// </summary>
        /// <remarks>
        /// This method reads the entire file into memory so it will be memory consuming for large files.
        /// This is not truly generic as it requires <typeparamref name="T"/> to be serializable. <typeparamref name="T"/> is only used to determine filename.
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="migration">An optional <see cref="Migration"/> for updating the contents of the file.</param>
        /// <returns>The deserialized contents of the file corresponding to <typeparamref name="T"/>.</returns>
        Task<T> ReadAsync<T>(Migration? migration = null);

        /// <summary>
        /// Reads from file for <typeparamref name="T"/>
        /// If the file is missing an instance is created using <paramref name="creator"/>.
        /// The created instance is then saved.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <param name="creator">
        /// A <see cref="Func{TResult}"/> that is used for creating an instance if the file is missing.
        /// </param>
        /// <param name="migration">An optional <see cref="Migration"/> for updating the contents of the file.</param>
        /// <returns>The deserialized contents of the file corresponding to <typeparamref name="T"/>.</returns>
        Task<T> ReadOrCreateAsync<T>(Func<T> creator, Migration? migration = null);

        /// <summary>
        /// Saves to a file for <typeparamref name="T"/>.
        /// The filename is typeof(T).Name and the extension specified in settings.
        /// <seealso cref="IRepository.Save{T}(System.IO.FileInfo, System.IO.FileInfo, T)"/>
        /// <remarks>
        /// This is not truly generic as it requires <typeparamref name="T"/> to be serializable. <typeparamref name="T"/> is only used to determine filename.
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveAsync<T>(T item);
    }
}