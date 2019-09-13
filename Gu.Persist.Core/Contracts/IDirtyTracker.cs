namespace Gu.Persist.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Tracks if files have c hanged since last save.
    /// </summary>
    public interface IDirtyTracker
    {
        /// <summary>
        /// Check if if <paramref name="item"/> has changed since last save.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fullFileName">The file name.</param>
        /// <param name="item">The item.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        /// <returns>True if <paramref name="item"/> has changed since last save.</returns>
        bool IsDirty<T>(string fullFileName, T item, IEqualityComparer<T> comparer);

        /// <summary>
        /// Store a deep copy for comparing with later.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fullFileName">The file name.</param>
        /// <param name="item">The item.</param>
        void Track<T>(string fullFileName, T item);

        /// <summary>
        /// Rename a tracked instance.
        /// </summary>
        /// <param name="oldName">The old file.</param>
        /// <param name="newName">The new file.</param>
        /// <param name="overWrite">Replace if exists.</param>
        void Rename(string oldName, string newName, bool overWrite);

        /// <summary>
        /// Remove all tracked items.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Remove a specific tracked item.
        /// </summary>
        /// <param name="fullFileName">The file.</param>
        void RemoveFromCache(string fullFileName);
    }
}