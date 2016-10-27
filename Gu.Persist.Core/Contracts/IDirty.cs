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
        bool IsDirty<T>(T item);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        bool IsDirty<T>(T item, IEqualityComparer<T> comparer);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        bool IsDirty<T>(T item, string fileName);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// Uses default comparer that serializes.
        /// </summary>
        bool IsDirty<T>(T item, FileInfo file);

        /// <summary>
        /// Compares serialized item to the last bytes saved to or read from file.
        /// </summary>
        bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer);
    }
}
