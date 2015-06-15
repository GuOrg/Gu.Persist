namespace Gu.Settings
{
    using System;

    /// <summary>
    /// Use this when you only want one setting per type.
    /// </summary>
    public interface IAutoRepository
    {
        RepositorySettings Settings { get; }
        
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