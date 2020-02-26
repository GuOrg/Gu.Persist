namespace Gu.Persist.Core
{
    /// <summary>
    /// Use this when you only want one setting per type.
    /// When using this it is important that T is the same when reading and saving.
    /// </summary>
    public interface IGenericRepository
    {
        /// <summary>
        /// Reads from file for <typeparamref name="T"/> and deserializes the contents to an instance of <typeparamref name="T"/>
        /// The filename is typeof(T).Name and the extension specified in settings.
        /// </summary>
        /// <remarks>
        /// This is not truly generic as it requires <typeparamref name="T"/> to be serializable. <typeparamref name="T"/> is only used to determine filename.
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <param name="migration">An optional <see cref="Migration"/> for updating the contents of the file.</param>
        /// <returns>The deserialized contents of the file.</returns>
        T Read<T>(Migration? migration = null);

        /// <summary>
        /// Serializes <paramref name="item"/> and saves to a file for <typeparamref name="T"/>.
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
        void Save<T>(T item);
    }
}