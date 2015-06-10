namespace Gu.Settings
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class FileHelper
    {
        internal static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public static void Delete(FileInfo fileInfo)
        {
            fileInfo.Delete();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="fromStream">Reading from from file to T</param>
        /// <returns></returns>
        internal static T Read<T>(FileInfo file, Func<Stream, T> fromStream, bool @lock = true)
        {
            if (@lock)
            {
                SemaphoreSlim.Wait();
            }
            try
            {
                using (var stream = File.OpenRead(file.FullName))
                {
                    return fromStream(stream);
                }
            }
            finally
            {
                if (@lock)
                {
                    SemaphoreSlim.Release();
                }
            }
        }

        /// <summary>
        /// Reads the contents of the file to a memorystream
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fromStream">Reading from stream to T</param>
        /// <returns></returns>
        internal static async Task<T> ReadAsync<T>(FileInfo file, Func<Stream, T> fromStream, bool @lock = true)
        {
            if (@lock)
            {
                await SemaphoreSlim.WaitAsync()
                                   .ConfigureAwait(false);
            }
            using (var ms = new MemoryStream())
            {
                try
                {
                    using (var stream = File.OpenRead(file.FullName))
                    {
                        await stream.CopyToAsync(ms)
                                    .ConfigureAwait(false);
                    }
                }
                finally
                {
                    if (@lock)
                    {
                        SemaphoreSlim.Release();
                    }
                }
                ms.Position = 0;
                return fromStream(ms);
            }
        }

        /// <summary>
        /// Generic method for saving a file async
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal static async Task SaveAsync(FileInfo file, Stream stream, bool @lock = true)
        {
            if (@lock)
            {
                await SemaphoreSlim.WaitAsync()
                                   .ConfigureAwait(false);
            }

            try
            {
                using (var fileStream = file.OpenWrite())
                {
                    await stream.CopyToAsync(fileStream)
                                .ConfigureAwait(false);
                }
            }
            finally
            {
                if (@lock)
                {
                    SemaphoreSlim.Release();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        internal static void Save(FileInfo file, Stream stream, bool @lock = true)
        {
            if (@lock)
            {
                SemaphoreSlim.Wait();
            }
            try
            {
                using (var fileStream = File.OpenWrite(file.FullName))
                {
                    stream.CopyTo(fileStream);
                }
            }
            finally
            {
                if (@lock)
                {
                    SemaphoreSlim.Release();
                }
            }
        }

        /// <summary>
        /// Creates the directory if not exists
        /// </summary>
        /// <param name="directory"></param>
        internal static void CreateDirectoryIfNotExists(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns>The name of the backed upp file</returns>
        internal static void Backup(FileInfos files, bool @lock = true)
        {
            if (@lock)
            {
                SemaphoreSlim.Wait();
            }
            try
            {
                if (files.Backup == null)
                {
                    return;
                }
                if (files.File.Exists)
                {
                    files.Backup.Delete();
                    files.File.MoveTo(files.Backup.FullName);
                }
            }
            finally
            {
                if (@lock)
                {
                    SemaphoreSlim.Release();
                }
            }
        }

        internal static void Restore(FileInfos files, bool @lock = true)
        {
            if (@lock)
            {
                SemaphoreSlim.Wait();
            }
            try
            {
                if (files.Backup == null)
                {
                    return;
                }
                if (!files.Backup.Exists)
                {
                    return;
                }
                files.File.Delete();
                files.Backup.MoveTo(files.File.FullName);
            }
            finally
            {
                if (@lock)
                {
                    SemaphoreSlim.Release();
                }
            }
        }

        internal static FileInfo CreateFileInfo(DirectoryInfo directory, string fileName, string extension)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            if (extension != null)
            {
                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }
                if (Path.HasExtension(fileName))
                {
                    var ext = Path.GetExtension(fileName);
                    if (!string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase))
                    {
                        fileName = Path.ChangeExtension(fileName, extension);
                    }
                }
                else
                {
                    fileName += extension;
                }
            }

            if (Path.IsPathRooted(fileName))
            {
                return new FileInfo(fileName);
            }

            var fullFileName = Path.Combine(directory.FullName, fileName);
            return new FileInfo(fullFileName);
        }
    }
}