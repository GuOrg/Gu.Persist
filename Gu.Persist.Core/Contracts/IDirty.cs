namespace Gu.Persist.Core
{
    using System.Collections.Generic;
    using System.IO;

    public interface IDirty
    {
        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// Uses default comparer that serializes and compares bytes.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>True if <paramref name="item"/> has changed since last saved.</returns>
        bool IsDirty<T>(T item);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        /// <returns>True if <paramref name="item"/> has changed since last saved.</returns>
        bool IsDirty<T>(T item, IEqualityComparer<T> comparer);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <param name="item">The item.</param>
        /// <returns>True if <paramref name="item"/> has changed since last saved.</returns>
        bool IsDirty<T>(string fileName, T item);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <param name="item">The item.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        /// <returns>True if <paramref name="item"/> has changed since last saved.</returns>
        bool IsDirty<T>(string fileName, T item, IEqualityComparer<T> comparer);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// Uses default comparer that serializes.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The item.</param>
        /// <returns>True if <paramref name="item"/> has changed since last saved.</returns>
        bool IsDirty<T>(FileInfo file, T item);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The item.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        /// <returns>True if <paramref name="item"/> has changed since last saved.</returns>
        bool IsDirty<T>(FileInfo file, T item, IEqualityComparer<T> comparer);
    }
}
