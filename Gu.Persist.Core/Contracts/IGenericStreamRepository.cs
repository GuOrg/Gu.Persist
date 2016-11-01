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
        /// Reads the file and returns the contents.
        /// </summary>
        /// <remarks>
        /// This overload is a poor fit for large files.
        /// </remarks>
        Stream Read<T>();

        /// <summary>
        /// Saves <paramref name="stream"/> to a file specified by <typeparamref name="T"/>
        /// <seealso cref="IRepository.Save{T}(T)"/>
        /// </summary>
        void Save<T>(Stream stream);
    }
}