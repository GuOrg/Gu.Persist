namespace Gu.Settings
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IRepository : ICloner, IDirty, IDisposable
    {
        IDirtyTracker Tracker { get; }
        
        IBackuper Backuper { get; }
        
        IRepositorySettings Settings { get; }

        /// <summary>
        /// This gets the fileinfo used for reading & writing files of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        FileInfo GetFileInfo<T>();

        /// <summary>
        /// Gets the fileinfo for that is used for the given filename
        /// </summary
        /// <param name="fileName"></param>
        /// <returns></returns>
        FileInfo GetFileInfo(string fileName);

        void Delete<T>(bool deleteBackups);

        void Delete(string fileName, bool deleteBackups);

        void Delete(FileInfo file, bool deleteBackups);

        bool Exists<T>();

        bool Exists(string fileName);

        bool Exists(FileInfo file);

        Task<T> ReadAsync<T>();

        Task<T> ReadAsync<T>(FileInfo file);

        T Read<T>();

        T ReadOrCreate<T>(Func<T> creator);

        T ReadOrCreate<T>(string fileName, Func<T> creator);

        T Read<T>(FileInfo file);

        T ReadOrCreate<T>(FileInfo file, Func<T> creator);

        /// <summary>
        /// Saves to a file named typeof(T).Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Save<T>(T item);

        void Save<T>(T item, FileInfo file);

        void Save<T>(T item, FileInfo file, FileInfo tempFile);

        Task SaveAsync<T>(T item);

        Task SaveAsync<T>(T item, string fileName);

        Task SaveAsync<T>(T item, FileInfo file);

        Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile);

        Task<T> ReadAsync<T>(string fileName);

        /// <summary>
        /// Reads from file the first time. After that it returns returns cached value (singleton).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Optional if blank a file with the name of the class is read.</param>
        /// <returns></returns>
        T Read<T>(string fileName);

        void Save<T>(T item, string fileName);

        bool CanRename<T>(string newName);

        void Rename<T>(string newName, bool owerWrite);

        bool CanRename(string oldName, string newName);

        void Rename(string oldName, string newName, bool owerWrite);

        bool CanRename(FileInfo oldName, string newName);

        void Rename(FileInfo oldName, string newName, bool owerWrite);

        bool CanRename(FileInfo oldName, FileInfo newName);

        void Rename(FileInfo oldName, FileInfo newName, bool owerWrite);
       
        void ClearCache();
        
        void ClearTrackerCache();

        /// <summary>
        /// Saves the file. Then removes it from cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fileName"></param>
        void SaveAndClose<T>(T item, string fileName);

        /// <summary>
        /// Saves the file. Then removes it from cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        void SaveAndClose<T>(T item, FileInfo file);

        void RemoveFromCache<T>(T item);

        void RemoveFromDirtyTracker<T>(T item);

        void DeleteBackups<T>();

        void DeleteBackups(string fileName);

        void DeleteBackups(FileInfo file);

        Task<MemoryStream> ReadStreamAsync<T>();

        Task<MemoryStream> ReadAsync(string fileName);

        Task<MemoryStream> ReadStreamAsync(FileInfo file);

        Stream ReadStream<T>();

        Stream Read(string fileName);

        Stream Read(FileInfo file);

        void SaveAndClose<T>(T item);

        void Save<T>(Stream stream);

        void Save(Stream stream, string fileName);

        /// <summary>
        /// Saves the stream and creates backups.
        /// </summary>
        /// <param name="stream">If the stream is null the file is deleted</param>
        /// <param name="file"></param>
        void Save(Stream stream, FileInfo file);

        void Save(Stream stream, FileInfo file, FileInfo tempFile);

        Task SaveAsync<T>(Stream stream);

        Task SaveAsync(Stream stream, string fileName);

        Task SaveAsync(Stream stream, FileInfo file);

        Task SaveAsync(Stream stream, FileInfo file, FileInfo tempFile);
    }
}