namespace Gu.Settings.Core
{
    using System;

    /// <summary>
    /// Use this when you only want one setting per type.
    /// When using this it is important that T is the same when reading and saving.
    /// Maybe Save(object o) and o.GetType() is better?
    /// </summary>
    public interface IGenericRepository
    {
        /// <see cref="IRepository.Delete{T}(bool)"/>
        void Delete<T>(bool deleteBackups);

        /// <see cref="IRepository.Exists{T}()"/>
        bool Exists<T>();

        /// <see cref="IRepository.Read{T}()"/>
        T Read<T>();

        /// <see cref="IRepository.ReadOrCreate{T}(Func{T})"/>
        T ReadOrCreate<T>(Func<T> creator);

        /// <see cref="IRepository.Read{T}()"/>
        void Save<T>(T item);

        /// <see cref="IRepository.IsDirty{T}(T)"/>
        bool IsDirty<T>(T item);

        /// <see cref="IRepository.Clone{T}(T)"/>
        T Clone<T>(T item);
    }
}