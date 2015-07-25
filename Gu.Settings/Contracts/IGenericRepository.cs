namespace Gu.Settings
{
    using System;

    /// <summary>
    /// Use this when you only want one setting per type.
    /// When using this it is important that T is the same when reading and saving.
    /// Maybe Save(object o) and o.GetType() is better?
    /// </summary>
    public interface IGenericRepository
    {
        void Delete<T>(bool deleteBackups);

        bool Exists<T>();

        /// <summary>
        /// Reads from file {Settings.Directory}/typeof(T).Name.{Settings.Extension}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Read<T>();

        /// <summary>
        /// Reads from file {Settings.Directory}/typeof(T).Name.{Settings.Extension}
        /// Creates and saves a new item if no file is found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ReadOrCreate<T>(Func<T> creator);

        /// <summary>
        /// Saves to file {Settings.Directory}/typeof(T).Name.{Settings.Extension}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Save<T>(T item);

        bool IsDirty<T>(T item);

        T Clone<T>(T item);
    }
}