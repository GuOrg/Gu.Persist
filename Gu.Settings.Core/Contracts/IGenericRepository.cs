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
        /// <summary>
        /// <see cref="IRepository.Delete{T}(bool)"/>
        /// </summary>
        void Delete<T>(bool deleteBackups);

        /// <summary>
        /// <see cref="IRepository.Exists{T}()"/>
        /// </summary>
        bool Exists<T>();

        /// <summary>
        /// <see cref="IRepository.Read{T}()"/>
        /// </summary>
        T Read<T>();

        /// <summary>
        ///  <see cref="IRepository.ReadOrCreate{T}(Func{T})"/>
        /// </summary>
        T ReadOrCreate<T>(Func<T> creator);

        /// <summary>
        /// <see cref="IRepository.Read{T}()"/>
        /// </summary>
        void Save<T>(T item);

        /// <summary>
        /// Check if <paramref name="item"/> has changes compared to when last saved.
        /// </summary>
        bool IsDirty<T>(T item);

        /// <summary>
        /// <see cref="ICloner.Clone{T}(T)"/>
        /// </summary>
        T Clone<T>(T item);
    }
}