namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// Use this for reading the raw streams.
    /// Not that caching and dirty tracking does not work for streams.
    /// </summary>
    public interface IGenericStreamRepository
    {
        /// <summary>
        /// Reads from file for <typeparamref name="T"/> and returns the contents.
        /// The filename is typeof(T).Name and the extension specified in settings.
        /// </summary>
        /// <remarks>
        /// This method reads the entire file into memory so it will be memory consuming for large files.
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <returns>The <see cref="Stream"/> read from the file.</returns>
        Stream Read<T>();

        /// <summary>
        /// Saves to a file for <typeparamref name="T"/>.
        /// The filename is typeof(T).Name and the extension specified in settings.
        /// </summary>
        /// <remarks>
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        /// <typeparam name="T">The type to save to the file.</typeparam>
        /// <param name="stream">The <see cref="Stream"/>.</param>
        void Save<T>(Stream stream);
    }
}