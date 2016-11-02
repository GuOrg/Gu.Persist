namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// Use this for reading the raw streams.
    /// Not that caching and dirty tracking does not work for streams.
    /// </summary>
    public interface IFileNameStreamRepository
    {
        /// <summary>
        /// Reads the file and returns the contents in a memorystream
        /// </summary>
        /// <remarks>
        /// This method reads the entire file into memory so it will be memory consuming for large files.
        /// When using streams no caching nor dirtytracking is performed.
        /// </remarks>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <returns>A stream with the contents of the file.</returns>
        Stream Read(string fileName);

        /// <summary>
        /// Saves <paramref name="stream"/> to a file specified by  <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <param name="stream">The stream to save.</param>
        void Save(string fileName, Stream stream);
    }
}