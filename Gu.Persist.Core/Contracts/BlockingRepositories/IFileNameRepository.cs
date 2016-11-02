#pragma warning disable 1573

namespace Gu.Persist.Core
{
    public interface IFileNameRepository
    {
        /// <summary>
        /// Reads from file or cache if caching.
        /// If caching every read will get the same singleton instance.
        /// Adds the instance to cache if caching.
        /// Starts tracking the if tracking
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        T Read<T>(string fileName);

        /// <summary>
        /// Saves the item to file.
        /// Adds it to cache if caching and first time it is read/saved
        /// Starts tracking if tracking changes.
        /// </summary>
        /// <typeparam name="T">Not used for anything, maybe useful for constraining?</typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        void Save<T>(string fileName, T item);
    }
}
