namespace Gu.Persist.Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// A repository that reads and saves async.
    /// </summary>
    public interface IFileNameAsyncRepository
    {
        /// <summary>
        /// Reads from <paramref name="fileName"/> and deserializes the contents.
        /// </summary>
        /// <remarks>
        /// This method reads the entire file into memory so it will be memory consuming for large files.
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <returns>The deserialized contents of the file corresponding to <typeparamref name="T"/>.</returns>
        Task<T> ReadAsync<T>(string fileName);

        /// <summary>
        /// Serialize <paramref name="item"/> and save to <paramref name="fileName"/>.
        /// </summary>
        /// <remarks>
        /// This is not truly generic as it requires <typeparamref name="T"/> to be serializable. <typeparamref name="T"/> is only used to determine filename.
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <param name="item">The instance to save.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveAsync<T>(string fileName, T item);
    }
}