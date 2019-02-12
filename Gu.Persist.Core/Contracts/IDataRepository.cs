namespace Gu.Persist.Core
{
    using System.IO;

    public interface IDataRepository : IRepository
    {
        /// <summary>
        /// Delete the file used for <typeparamref name="T"/>.
        /// </summary>
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
        void Delete(FileInfo file, bool deleteBackups);
    }
}