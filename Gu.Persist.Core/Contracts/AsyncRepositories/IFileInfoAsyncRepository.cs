namespace Gu.Persist.Core
{
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
        /// <returns>The deserialized contents of <paramref name="file"/>.</returns>
        Task<T> ReadAsync<T>(FileInfo file);

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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveAsync<T>(FileInfo file, T item);
    }
}