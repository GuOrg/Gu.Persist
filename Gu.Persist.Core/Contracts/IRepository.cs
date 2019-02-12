#pragma warning disable 1573
namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// This interface should probably bot be exposed as it contains everything.
    /// There are smaller interfaces that provide better API for most situations.
    /// </summary>
    public interface IRepository : ICloner, IDirty
    {
        /// <summary>
        /// Gets the <see cref="IDirtyTracker"/> that tracks if the file is dirty i.e. has changes since last save.
        /// </summary>
        IDirtyTracker Tracker { get; }

        /// <summary>
        /// Gets the <see cref="IBackuper"/> that handles backup files.
        /// </summary>
        IBackuper Backuper { get; }

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

        /// <summary>
        /// Reads from file. The filename is <typeparamref name="T"/>.Name.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        Task<T> ReadAsync<T>();

        /// <summary>
        /// Reads from file specified by <paramref name="fileName"/>.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        Task<T> ReadAsync<T>(string fileName);

        /// <summary>
        /// <see cref="IFileInfoAsyncRepository.ReadAsync{T}(FileInfo)"/>.
        /// </summary>
        Task<T> ReadAsync<T>(FileInfo file);

        /// <summary>
        /// Reads from file for <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        T Read<T>();

        /// <summary>
        /// Reads from file for <typeparamref name="T"/>
        /// If the file is missing an instance is created using <paramref name="creator"/>.
        /// The created instance is then saved.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <param name="creator">
        /// A <see cref="Func{TResult}"/> that is used for creating an instance if the file is missing.
        /// </param>
        T ReadOrCreate<T>(Func<T> creator);

        /// <summary>
        /// Reads from file specified by <paramref name="fileName"/>.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        T Read<T>(string fileName);

        /// <summary>
        /// Reads from file specified by <paramref name="fileName"/>.
        /// If the file is missing an instance is created using <paramref name="creator"/>.
        /// The created instance is then saved.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <param name="creator">
        /// A <see cref="Func{TResult}"/> that is used for creating an instance if the file is missing.
        /// </param>
        T ReadOrCreate<T>(string fileName, Func<T> creator);

        /// <summary>
        /// Reads the contents of <paramref name="file"/> and deserializes it to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        T Read<T>(FileInfo file);

        /// <summary>
        /// Reads from file specified by <paramref name="file"/>.
        /// If the file is missing an instance is created using <paramref name="creator"/>.
        /// The created instance is then saved.
        /// </summary>
        T ReadOrCreate<T>(FileInfo file, Func<T> creator);

        /// <summary>
        /// Saves to a file for <typeparamref name="T"/>.
        /// <seealso cref="IRepository.Save{T}(FileInfo, FileInfo, T)"/>
        /// </summary>
        void Save<T>(T item);

        /// <summary>
        /// Saves to a file specified by <paramref name="fileName"/>.
        /// <seealso cref="IRepository.Save{T}(FileInfo, FileInfo, T)"/>
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        void Save<T>(string fileName, T item);

        /// <summary>
        /// Saves <see paramref="item"/> to <paramref name="file"/>.
        /// <seealso cref="IRepository.Save{T}(FileInfo, FileInfo, T)"/>
        /// </summary>
        void Save<T>(FileInfo file, T item);

        /// <summary>
        /// 1) Creates backup if specified in settings.
        /// 2) Renames <paramref name="file"/> with suffix .delete
        /// 3) Saves <paramref name="item"/> to <paramref name="tempFile"/>
        /// 4 a)If save is successful <paramref name="tempFile"/> is renamed to <paramref name="file"/>.
        /// </summary>
        /// <param name="tempFile">The file to use as temporary file when saving.</param>
        void Save<T>(FileInfo file, FileInfo tempFile, T item);

        /// <summary>
        /// <see cref="IRepository.Save{T}(T)"/>.
        /// </summary>
        Task SaveAsync<T>(T item);

        /// <summary>
        /// <see cref="IRepository.Save{T}(string,T)"/>.
        /// </summary>
        Task SaveAsync<T>(string fileName, T item);

        /// <summary>
        /// <see cref="IRepository.Save{T}(FileInfo,T)"/>.
        /// </summary>
        Task SaveAsync<T>(FileInfo file, T item);

        /// <summary>
        /// <see cref="IRepository.Save{T}(FileInfo, FileInfo, T)"/>.
        /// </summary>
        Task SaveAsync<T>(FileInfo file, FileInfo tempFile, T item);

        /// <summary>
        /// Checks if the file for <typeparamref name="T"/> can be renamed to <paramref name="newName"/>.
        /// </summary>
        bool CanRename<T>(string newName);

        /// <summary>
        /// Renames the file for <typeparamref name="T"/> to <paramref name="newName"/>.
        /// </summary>
        /// <param name="overWrite">If true the destination file is overwritten if it exists.</param>
        void Rename<T>(string newName, bool overWrite);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>.
        /// </summary>
        bool CanRename(string oldName, string newName);

        /// <summary>
        /// Renames the file <paramref name="oldName"/> to <paramref name="newName"/>.
        /// </summary>
        /// <param name="overWrite">If true the destination file is overwritten if it exists.</param>
        void Rename(string oldName, string newName, bool overWrite);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>.
        /// </summary>
        bool CanRename(FileInfo oldName, string newName);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>.
        /// </summary>
        /// <param name="overWrite">If true the destination file is overwritten if it exists.</param>
        void Rename(FileInfo oldName, string newName, bool overWrite);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>.
        /// </summary>
        bool CanRename(FileInfo oldName, FileInfo newName);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>.
        /// </summary>
        /// <param name="overWrite">If true the destination file is overwritten if it exists.</param>
        void Rename(FileInfo oldName, FileInfo newName, bool overWrite);

        /// <summary>
        /// Clears the cache of the <see cref="IDirtyTracker"/>.
        /// </summary>
        void ClearTrackerCache();

        /// <summary>
        /// Removes <paramref name="item"/> from the <see cref="IDirtyTracker"/> cache.
        /// </summary>
        void RemoveFromDirtyTracker<T>(T item);

        /// <summary>
        /// Deletes the backups for the file specified by. <typerparamref name="T"/>
        /// </summary>
        void DeleteBackups<T>();

        /// <summary>
        /// Deletes the backups for the file specified by <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        void DeleteBackups(string fileName);

        /// <summary>
        /// Deletes backups for <paramref name="file"/>.
        /// </summary>
        void DeleteBackups(FileInfo file);
    }
}