﻿namespace Gu.Persist.Core
{
    /// <summary>
    /// A <see cref="Repository{TSetting}"/> with API using file names.
    /// </summary>
    public interface IFileNameRepository
    {
        /// <summary>
        /// Reads from <paramref name="fileName"/> and deserializes the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
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
        /// <param name="migration">An optional <see cref="Migration"/> for updating the contents of the file.</param>
        /// <returns>The deserialized contents of the file.</returns>
        T Read<T>(string fileName, Migration? migration = null);

        /// <summary>
        /// Serializes <paramref name="item"/> and saves to <paramref name="fileName"/>.
        /// </summary>
        /// <remarks>
        /// If the repository is a <see cref="ISingletonRepository"/> the repository manages a singleton instance that is returned in future reads.
        /// Also for <see cref="ISingletonRepository"/> a check is made to ensure that the same instance is saved.
        /// </remarks>
        /// <typeparam name="T">The type of the instance to save.</typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <param name="item">The item.</param>
        void Save<T>(string fileName, T item);
    }
}
