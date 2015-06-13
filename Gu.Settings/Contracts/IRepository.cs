namespace Gu.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface IRepository : ICloner, IDirty
    {
        RepositorySetting Setting { get; }
       
        IDirtyTracker Tracker { get; }
        
        IBackuper Backuper { get; }

        bool Exists<T>();

        bool Exists<T>(string fileName);

        bool Exists<T>(FileInfo file);

        Task<T> ReadAsync<T>();

        Task<T> ReadAsync<T>(string fileName);

        Task<T> ReadAsync<T>(FileInfo file);

        T Read<T>();

        T ReadOrCreate<T>(Func<T> creator);

        /// <summary>
        /// Reads from file the first time. After that it returns returns cached value (singleton).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Optional if blank a file with the name of the class is read.</param>
        /// <returns></returns>
        T Read<T>(string fileName);

        T ReadOrCreate<T>(string fileName, Func<T> creator);

        T Read<T>(FileInfo file);

        T ReadOrCreate<T>(FileInfo file, Func<T> creator);

        /// <summary>
        /// Saves to a file named typeof(T).Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Save<T>(T item);

        void Save<T>(T item, string fileName);

        void Save<T>(T item, FileInfo file);

        void Save<T>(T item, FileInfo file, FileInfo tempFile);

        Task SaveAsync<T>(T item);

        Task SaveAsync<T>(T item, string fileName);

        Task SaveAsync<T>(T item, FileInfo file);

        Task SaveAsync<T>(T item, FileInfo file, FileInfo tempFile);

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        void Dispose();

        /// <summary>
        /// This gets the fileinfo used for reading & writing files of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        FileInfo GetFileInfo<T>();
    }
}