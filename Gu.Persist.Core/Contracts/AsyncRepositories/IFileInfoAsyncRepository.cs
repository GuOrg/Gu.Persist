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
        Task<T> ReadAsync<T>(FileInfo file);

        /// <summary>
        /// Serialize <paramref name="item"/> and save to <paramref name="file"/>.
        /// </summary>
        /// <remarks>
        /// This is not truly generic as it requires <typeparamref name="T"/> to be serializable. <typeparamref name="T"/> is only used to determine filename.
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        Task SaveAsync<T>(FileInfo file, T item);
    }
}