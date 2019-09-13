namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// Use this for reading the raw streams.
    /// When using streams no caching nor dirtytracking is performed.
    /// </summary>
    public interface IStreamRepository :
        IGenericStreamRepository,
        IGenericAsyncStreamRepository,
        IFileNameStreamRepository,
        IFileNameAsyncStreamRepository,
        IFileInfoStreamRepository,
        IFileInfoAsyncStreamRepository
    {
        /// <summary>
        /// Gets the settings used by the repository.
        /// </summary>
        IRepositorySettings Settings { get; }

        /// <summary>
        /// This gets the <see cref="FileInfo"/> used for reading and writing files of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        FileInfo GetFileInfo<T>();

        /// <summary>
        /// Gets the <see cref="FileInfo"/> for that is used for the given filename.
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        FileInfo GetFileInfo(string fileName);

        /// <summary>
        /// Check if the file for <typeparamref name="T"/> exists.
        /// </summary>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        bool Exists<T>();

        /// <summary>
        /// Check if the file for <paramref  name="fileName"/> exists.
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        bool Exists(string fileName);

        /// <summary>
        /// Check if the file exists.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        bool Exists(FileInfo file);
    }
}