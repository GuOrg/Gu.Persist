namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository that reads and saves async.
    /// </summary>
    public interface IFileInfoAsyncRepository
    {
        /// <summary>
        /// Reads from <paramref name="file"/> and deserializes the contents.
        /// </summary>
        /// <remarks>
        /// This method reads the entire file into memory so it will be memory consuming for large files.
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="migration">An optional <see cref="Migration"/> for updating the contents of the file.</param>
        /// <returns>The deserialized contents of <paramref name="file"/>.</returns>
        Task<T> ReadAsync<T>(FileInfo file, Migration? migration = null);

        /// <summary>
        /// Reads from file specified by <paramref name="file"/>.
        /// If the file is missing an instance is created using <paramref name="creator"/>.
        /// The created instance is then saved.
        /// </summary>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="creator">The <see cref="Func{T}"/>.</param>
        /// <param name="migration">An optional <see cref="Migration"/> for updating the contents of the file.</param>
        /// <returns>The deserialized contents of <paramref name="file"/>.</returns>
        Task<T> ReadOrCreateAsync<T>(FileInfo file, Func<T> creator, Migration? migration = null);

        /// <summary>
        /// Serialize <paramref name="item"/> and save to <paramref name="file"/>.
        /// </summary>
        /// <remarks>
        /// This is not truly generic as it requires <typeparamref name="T"/> to be serializable. <typeparamref name="T"/> is only used to determine filename.
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The item.</param>
        /// <returns>A <see cref="Task"/> representing the save operation.</returns>
        Task SaveAsync<T>(FileInfo file, T item);
    }
}