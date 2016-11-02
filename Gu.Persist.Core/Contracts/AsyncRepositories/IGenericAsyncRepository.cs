namespace Gu.Persist.Core
{
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
        Task<T> ReadAsync<T>();

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
        Task SaveAsync<T>(T item);
    }
}