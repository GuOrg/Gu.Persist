namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// A repository for async reads and writes.
    /// </summary>
    public interface IAsyncRepository : IFileInfoAsyncRepository, IFileNameAsyncRepository, IGenericAsyncRepository, ICloner, IDirty
    {
        /// <summary>
        /// Gets the settings used by the repository.
        /// </summary>
        IRepositorySettings Settings { get; }

        /// <summary>
        /// This gets the fileinfo used for reading and writing files of type <typeparamref name="T"/>.
        /// </summary>
        FileInfo GetFileInfo<T>();

        /// <summary>
        /// Gets the fileinfo for that is used for the given filename.
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
        bool Exists(FileInfo file);
    }
}