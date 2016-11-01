namespace Gu.Persist.Core
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IStreamRepository
    {
        /// <summary>
        /// Reads the file and returns the contents in a memorystream
        /// </summary>
        /// <remarks>
        /// This overload is a poor fit for large files.
        /// </remarks>
        Stream Read<T>();

        /// <summary>
        /// Reads the file and returns the contents in a memorystream
        /// </summary>
        /// <remarks>
        /// This overload is a poor fit for large files.
        /// </remarks>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        Stream Read(string fileName);

        /// <summary>
        /// Reads the file <paramref name="file"/> and returns the contents in a memorystream
        /// </summary>
        Stream Read(FileInfo file);

        /// <summary>
        /// <see cref="Read{T}"/>
        /// </summary>
        Task<Stream> ReadAsync<T>();

        /// <summary>
        /// <see cref="Read(string)"/>
        /// </summary>
        Task<Stream> ReadAsync(string fileName);

        /// <summary>
        ///  <see cref="Read(FileInfo)"/>
        /// </summary>
        Task<Stream> ReadAsync(FileInfo file);

        /// <summary>
        /// Saves <paramref name="stream"/> to a file specified by <typeparamref name="T"/>
        /// <seealso cref="IRepository.Save{T}(T)"/>
        /// </summary>
        void Save<T>(Stream stream);

        /// <summary>
        /// Saves <paramref name="stream"/> to a file specified by  <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        void Save(string fileName, Stream stream);

        /// <summary>
        /// Saves the stream and creates backups.
        /// </summary>
        void Save(FileInfo file, Stream stream);

        /// <summary>
        /// Saves <paramref name="stream"/> to <paramref name="file"/>
        /// <seealso cref="IRepository.Save{T}(FileInfo, FileInfo, T)"/>
        /// </summary>
        void Save(FileInfo file, FileInfo tempFile, Stream stream);

        /// <summary>
        /// <see cref="IRepository.SaveStream{T}(Stream)"/>
        /// </summary>
        Task SaveAsync<T>(Stream stream);

        /// <summary>
        /// <see cref="IRepository.SaveStream(string, Stream)"/>
        /// </summary>
        Task SaveAsync(string fileName, Stream stream);

        /// <summary>
        /// <see cref="IRepository.SaveStream(FileInfo,Stream)"/>
        /// </summary>
        Task SaveAsync(FileInfo file, Stream stream);

        /// <summary>
        /// <see cref="IRepository.SaveStream(FileInfo, FileInfo, Stream)"/>
        /// </summary>
        Task SaveAsync(FileInfo file, FileInfo tempFile, Stream stream);

    }
}