namespace Gu.Settings
{
    using System.Collections.Generic;
    using System.IO;

    public interface IDirty
    {
        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// Uses default comparer that serializes and compares bytes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IsDirty<T>(T item);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        bool IsDirty<T>(T item, IEqualityComparer<T> comparer);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool IsDirty<T>(T item, string fileName);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fileName"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// Uses default comparer that serializes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        bool IsDirty<T>(T item, FileInfo file);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="file"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer);
    }
}
