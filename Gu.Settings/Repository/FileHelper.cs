namespace Gu.Settings
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class FileHelper
    {
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="fromStream">Reading from from file to T</param>
        /// <returns></returns>
        internal static T Read<T>(FileInfo file, Func<Stream, T> fromStream)
        {
            SemaphoreSlim.Wait();
            try
            {
                using (var stream = File.OpenRead(file.FullName))
                {
                    return fromStream(stream);
                }
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Reads the contents of the file to a memorystream
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fromStream">Reading from stream to T</param>
        /// <returns></returns>
        internal static async Task<T> ReadAsync<T>(FileInfo file, Func<Stream, T> fromStream)
        {
            await SemaphoreSlim.WaitAsync()
                               .ConfigureAwait(false);
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
                    SemaphoreSlim.Release();
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
        internal static async Task SaveAsync(FileInfo file, Stream stream)
        {
            await SemaphoreSlim.WaitAsync()
                               .ConfigureAwait(false);
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
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        internal static void Save(FileInfo file, Stream stream)
        {
            SemaphoreSlim.Wait();
            try
            {
                using (var fileStream = File.OpenWrite(file.FullName))
                {
                    stream.CopyTo(fileStream);
                }
            }
            finally
            {
                SemaphoreSlim.Release();
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
        internal static void Backup(FileInfos files)
        {
            if (files.Backup == null)
            {
                return;
            }
            if (files.File.Exists)
            {
                if (files.Backup.Exists)
                {
                    files.Backup.Delete();
                }
                files.File.MoveTo(files.Backup.FullName);
            }
        }

        internal static void Restore(FileInfos files)
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
    }
}