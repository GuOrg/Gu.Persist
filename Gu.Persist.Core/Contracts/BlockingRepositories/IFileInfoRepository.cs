namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// A <see cref="Repository{TSetting}"/> with API using <see cref="FileInfo"/>.
    /// </summary>
    public interface IFileInfoRepository
    {
        /// <summary>
        /// Reads from <paramref name="file"/> and deserializes the contents.
        /// </summary>
        /// <remarks>
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <returns>The deserialized contents of the file.</returns>
        T Read<T>(FileInfo file);

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
        void Save<T>(FileInfo file, T item);
    }
}