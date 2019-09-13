namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// A repository for sync reads and writes.
    /// </summary>
    public interface IBlockingRepository : IFileInfoRepository, IFileNameRepository, IGenericRepository, ICloner, IDirty
    {
        /// <summary>
        /// Gets the settings used by the repository.
        /// </summary>
        IRepositorySettings Settings { get; }

        /// <summary>
        /// This gets the <see cref="FileInfo"/> used for reading and writing files of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <returns>A <see cref="FileInfo"/>.</returns>
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
        /// <returns>A <see cref="FileInfo"/>.</returns>
        FileInfo GetFileInfo(string fileName);

        /// <summary>
        /// Check if the file for <typeparamref name="T"/> exists.
        /// </summary>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <returns>True if the file exists.</returns>
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
        /// <returns>True if the file exists.</returns>
        bool Exists(string fileName);

        /// <summary>
        /// Check if the file exists.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <returns>True if the file exists.</returns>
        bool Exists(FileInfo file);
    }
}