namespace Gu.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFileNameRepository
    {
        RepositorySettings Settings { get; }

        bool Exists<T>(string fileName);

        Task<T> ReadAsync<T>(string fileName);

        /// <summary>
        /// Reads from file the first time. After that it returns returns cached value (singleton).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Optional if blank a file with the name of the class is read.</param>
        /// <returns></returns>
        T Read<T>(string fileName);

        T ReadOrCreate<T>(string fileName, Func<T> creator);

        void Save<T>(T item, string fileName);

        Task SaveAsync<T>(T item, string fileName);

        bool IsDirty<T>(T item, string fileName);

        bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer);

        T Clone<T>(T item);
    }
}
