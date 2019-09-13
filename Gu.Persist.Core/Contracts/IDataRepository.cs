namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// A repository that does not cache reads nor track dirty.
    /// </summary>
    public interface IDataRepository : IRepository
    {
        /// <summary>
        /// Delete the file used for <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <param name="deleteBackups">If true backup files are deleted.</param>
        void Delete<T>(bool deleteBackups);

        /// <summary>
        /// Delete the file if it exists.
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName.
        /// </param>
        /// <param name="deleteBackups">If true backups are deleted.</param>
        void Delete(string fileName, bool deleteBackups);

        /// <summary>
        /// Delete the file.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="deleteBackups">If backups should be deleted.</param>
        void Delete(FileInfo file, bool deleteBackups);
    }
}