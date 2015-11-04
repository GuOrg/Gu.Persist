namespace Gu.Settings.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// This interface should probably bot be exposed as it contains everythying.
    /// </summary>
    public interface IRepository : ICloner, IDirty
    {
        /// <summary>
        /// Gets the <see cref="IDirtyTracker"/> that tracks if the file is dirty i.e. has changes since last save.
        /// </summary>
        IDirtyTracker Tracker { get; }
        
        /// <summary>
        /// Gets the <see cref="IBackuper"/> that handles backup files
        /// </summary>
        IBackuper Backuper { get; }
        
        /// <summary>
        /// Gets the settings used by the repository.
        /// </summary>
        IRepositorySettings Settings { get; }

        /// <summary>
        /// This gets the fileinfo used for reading & writing files of type <typeparam name="T"></typeparam>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        FileInfo GetFileInfo<T>();

        /// <summary>
        /// Gets the fileinfo for that is used for the given filename
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <returns></returns>
        FileInfo GetFileInfo(string fileName);

        /// <summary>
        /// Delete the file used for <typeparamref name="T"/>
        /// </summary>
        /// <param name="deleteBackups">If true backup files are deleted.</param>
        void Delete<T>(bool deleteBackups);

        /// <summary>
        /// Delete the file if it exists.
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <param name="deleteBackups">If true backups are deleted</param>
        /// <returns></returns>
        void Delete(string fileName, bool deleteBackups);

        /// <summary>
        /// Delete the file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="deleteBackups"></param>
        void Delete(FileInfo file, bool deleteBackups);

        /// <summary>
        /// Check if the file for <typeparamref name="T"/> exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool Exists<T>();

        /// <summary>
        /// Check if the file for <paramref  name="fileName"/> exists
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <returns></returns>
        bool Exists(string fileName);
        
        /// <summary>
        /// Check if the file exists
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool Exists(FileInfo file);

        /// <summary>
        /// Reads from file. The filename is <typeparamref name="T"/>.Name
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> ReadAsync<T>();

        /// <summary>
        /// Reads from file specified by <paramref name="fileName"/>.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <returns></returns>
        Task<T> ReadAsync<T>(string fileName);

        /// <see cref="IRepository.Read{T}(FileInfo)"/>
        Task<T> ReadAsync<T>(FileInfo file);

        /// <summary>
        /// Reads from file for <typeparamref name="T"/>
        /// </summary>
        ///  <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Read<T>();

        /// <summary>
        /// Reads from file for <typeparamref name="T"/>
        /// If the file is missing an instance is created using <paramref name="creator"/>. 
        /// The created instance is then saved.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="creator"></param>
        /// <returns></returns>
        T ReadOrCreate<T>(Func<T> creator);

        /// <summary>
        /// Reads from file specified by <paramref name="fileName"/>.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <returns></returns>
        T Read<T>(string fileName);

        /// <summary>
        /// Reads from file specified by <paramref name="fileName"/>.
        /// If the file is missing an instance is created using <paramref name="creator"/>. 
        /// The created instance is then saved.
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <param name="creator"></param>
        /// <returns></returns>
        T ReadOrCreate<T>(string fileName, Func<T> creator);

        /// <summary>
        /// Reads the contents of <paramref name="file"/> and deserializes it to an instance of <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// If caching is enabled the repository manages a singleton instance that is returned on future reads.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        T Read<T>(FileInfo file);

        /// <summary>
        /// Reads from file specified by <paramref name="file"/>.
        /// If the file is missing an instance is created using <paramref name="creator"/>. 
        /// The created instance is then saved.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        T ReadOrCreate<T>(FileInfo file, Func<T> creator);


        /// <see cref="IRepository.ReadStream{T}()"/>
        Task<MemoryStream> ReadStreamAsync<T>();

        /// <see cref="IRepository.ReadStream(string)"/>
        Task<MemoryStream> ReadStreamAsync(string fileName);

        /// <see cref="IRepository.ReadStream(FileInfo)"/>
        Task<MemoryStream> ReadStreamAsync(FileInfo file);

        /// <summary>
        /// Reads the file and returns the contents in a memorystream
        /// </summary>
        /// <remarks>
        /// This overload is a poor fit for large files.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Stream ReadStream<T>();

        /// <summary>
        /// Reads the file and returns the contents in a memorystream
        /// </summary>
        /// <remarks>
        /// This overload is a poor fit for large files.
        /// </remarks>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <returns></returns>
        Stream ReadStream(string fileName);

        /// <summary>
        /// Reads the file <paramref name="file"/> and returns the contents in a memorystream
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Stream ReadStream(FileInfo file);

        /// <summary>
        /// Saves to a file for <typeparamref name="T"/>
        /// <seealso cref="IRepository.Save{T}(T, FileInfo, FileInfo)"/>
        /// </summary>
        /// <remarks>
        /// If <paramref name="item"/> is null the file is deleted.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Save<T>(T item);

        /// <summary>
        /// Saves to a file specified by <paramref name="fileName"/>.
        /// <seealso cref="IRepository.Save{T}(T, FileInfo, FileInfo)"/>
        /// </summary>
        /// <remarks>
        /// If <paramref name="item"/> is null the file is deleted.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        void Save<T>(T item, string fileName);

        /// <summary>
        /// Saves <see paramref="item"/> to <paramref name="file"/>
        /// <seealso cref="IRepository.Save{T}(T, FileInfo, FileInfo)"/>
        /// </summary>
        /// <remarks>
        /// If <paramref name="item"/> is null the file is deleted.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        void Save<T>(T item, FileInfo file);

        /// <summary>
        /// 1) Creates backup if specified in settings.
        /// 2) Renames <paramref name="file"/> with suffix .delete
        /// 3) Saves <paramref name="item"/> to <paramref name="tempFile"/>
        /// 4 a)If save is successful <paramref name="tempFile"/> is renamed to <paramref name="file"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        /// <param name="tempFile">The file to use as temporayr file when saving</param>
        void Save<T>(T item, FileInfo file, FileInfo tempFile);

        /// <see cref="IRepository.Save{T}(T)"/>
        Task SaveAsync<T>(T item);

        /// <see cref="IRepository.Save{T}(T, string)"/>
        Task SaveAsync<T>(T item, string fileName);

        /// <see cref="IRepository.Save{T}(T, FileInfo)"/>
        Task SaveAsync<T>(T item, FileInfo file);

        /// <see cref="IRepository.Save{T}(T, FileInfo, FileInfo)"/>
        Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile);

        /// <summary>
        /// Saves <paramref name="stream"/> to a file specified by <typeparamref name="T"/>
        /// <seealso cref="IRepository.Save{T}(T, FileInfo, FileInfo)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        void SaveStream<T>(Stream stream);

        /// <summary>
        /// Saves <paramref name="stream"/> to a file specified by  <paramref name="fileName"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        void SaveStream(Stream stream, string fileName);

        /// <summary>
        /// Saves the stream and creates backups.
        /// </summary>
        /// <param name="stream">If the stream is null the file is deleted</param>
        /// <param name="file"></param>
        void SaveStream(Stream stream, FileInfo file);

        /// <summary>
        /// Saves <paramref name="stream"/> to <paramref name="file"/>
        /// <seealso cref="IRepository.Save{T}(T, FileInfo, FileInfo)"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="file"></param>
        /// <param name="tempFile"></param>
        void SaveStream(Stream stream, FileInfo file, FileInfo tempFile);

        /// <see cref="IRepository.SaveStream{T}(Stream)"/>
        Task SaveStreamAsync<T>(Stream stream);

        /// <see cref="IRepository.SaveStream(Stream, string)"/>
        Task SaveStreamAsync(Stream stream, string fileName);

        /// <see cref="IRepository.SaveStream(Stream, FileInfo)"/>
        Task SaveStreamAsync(Stream stream, FileInfo file);

        /// <see cref="IRepository.SaveStream(Stream, FileInfo, FileInfo)"/>
        Task SaveStreamAsync(Stream stream, FileInfo file, FileInfo tempFile);

        /// <summary>
        /// Saves <paramref name="item"/> the file specified by <typeparamref name="T"/>. Then removes it from cache.
        /// <seealso cref="IRepository.Save{T}(T, string)"/>
        /// <seealso cref="IRepository.RemoveFromCache{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void SaveAndClose<T>(T item);

        /// <summary>
        /// Saves <paramref name="item"/> the file specified by <paramref name="fileName"/>. Then removes it from cache.
        /// <seealso cref="IRepository.Save{T}(T, string)"/>
        /// <seealso cref="IRepository.RemoveFromCache{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        void SaveAndClose<T>(T item, string fileName);

        /// <summary>
        /// Saves the file. Then removes it from cache.
        /// <seealso cref="IRepository.Save{T}(T, string)"/>
        /// <seealso cref="IRepository.RemoveFromCache{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        void SaveAndClose<T>(T item, FileInfo file);

        /// <summary>
        /// Checks if the file for <typeparamref name="T"/> can be renamed to <paramref name="newName"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newName"></param>
        /// <returns></returns>
        bool CanRename<T>(string newName);

        /// <summary>
        /// Renames the file for <typeparamref name="T"/> to <paramref name="newName"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newName"></param>
        /// <param name="owerWrite">If true the destination file is owerwritten if it exists.</param>
        void Rename<T>(string newName, bool owerWrite);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        bool CanRename(string oldName, string newName);

        /// <summary>
        /// Renames the file <paramref name="oldName"/> to <paramref name="newName"/>
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="owerWrite">If true the destination file is owerwritten if it exists.</param>
        /// <returns></returns>
        void Rename(string oldName, string newName, bool owerWrite);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        bool CanRename(FileInfo oldName, string newName);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="owerWrite">If true the destination file is owerwritten if it exists.</param>
        /// <returns></returns>
        void Rename(FileInfo oldName, string newName, bool owerWrite);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        bool CanRename(FileInfo oldName, FileInfo newName);

        /// <summary>
        /// Checks if <paramref name="oldName"/> can be renamed to <paramref name="newName"/>
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="owerWrite">If true the destination file is owerwritten if it exists.</param>
        /// <returns></returns>
        void Rename(FileInfo oldName, FileInfo newName, bool owerWrite);
       
        /// <summary>
        /// Clears the cache.
        ///  </summary>
        /// <remarks>
        /// Calling this means that singletons will no longer be resturned by the repository
        /// </remarks>
        void ClearCache();
        
        /// <summary>
        /// Clears the cache of the <see cref="IDirtyTracker"/>
        /// </summary>
        void ClearTrackerCache();

        /// <summary>
        /// Removes <paramref name="item"/> from cache.
        /// Next read will read a new instance from disk.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void RemoveFromCache<T>(T item);

        /// <summary>
        /// Removes <paramref name="item"/> from the <see cref="IDirtyTracker"/> cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void RemoveFromDirtyTracker<T>(T item);

        /// <summary>
        /// Deletes the backups for the file specified by <typerparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void DeleteBackups<T>();

        /// <summary>
        /// Deletes the backups for the file specified by <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        void DeleteBackups(string fileName);

        /// <summary>
        /// Deletes backups for <paramref name="file"/>
        /// </summary>
        /// <param name="file"></param>
        void DeleteBackups(FileInfo file);
    }
}