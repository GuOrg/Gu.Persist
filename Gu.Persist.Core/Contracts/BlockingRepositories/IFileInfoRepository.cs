namespace Gu.Persist.Core
{
    using System.IO;

    public interface IFileInfoRepository
    {
        /// <summary>
        /// Reads from <paramref name="file"/> and deserializes the contents.
        /// </summary>
        /// <remarks>
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        T Read<T>(FileInfo file);

        /// <summary>
        /// Serialize <paramref name="item"/> and save to <paramref name="file"/>.
        /// </summary>
        /// <remarks>
        /// This is not truly generic as it requires <typeparamref name="T"/> to be serializable. <typeparamref name="T"/> is only used to determine filename.
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        void Save<T>(FileInfo file, T item);
    }
}