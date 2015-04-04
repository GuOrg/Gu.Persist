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
        /// <param name="fullFileName"></param>
        /// <param name="fromStream">Deserialization from stream to T</param>
        /// <param name="throwIfNotExists">Throws and exception if file is missing when true.
        /// If false it returns default(T)</param>
        /// <returns></returns>
        internal static T Read<T>(string fullFileName, Func<Stream, T> fromStream, bool throwIfNotExists)
        {
            SemaphoreSlim.Wait();
            try
            {
                using (var stream = File.OpenRead(fullFileName))
                {
                    return fromStream(stream);
                }
            }
            catch (Exception)
            {
                if (throwIfNotExists)
                {
                    throw;
                }
                return default(T);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Reads the contents of the file to a memorystream
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="fromStream">Deserialization from stream to T</param>
        /// <param name="throwIfNotExists">Throws and exception if file is missing when true.
        /// If false it returns default(T)</param>
        /// <returns></returns>
        internal static async Task<T> ReadAsync<T>(string fullFileName, Func<Stream, T> fromStream, bool throwIfNotExists)
        {
            await SemaphoreSlim.WaitAsync().ConfigureAwait(false);
            using (var ms = new MemoryStream())
            {
                try
                {
                    using (var stream = File.OpenRead(fullFileName))
                    {
                        await stream.CopyToAsync(ms)
                                    .ConfigureAwait(false);
                    }
                }
                catch (Exception)
                {
                    if (throwIfNotExists)
                    {
                        throw;
                    }
                    return default(T);
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
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="fullFileName"></param>
        /// <param name="toStream">The function to convert o to a stream</param>
        /// <returns></returns>
        internal static async Task SaveAsync<T>(T o, string fullFileName, Func<T, Stream> toStream)
        {
            using (var ms = toStream(o))
            {
                await SemaphoreSlim.WaitAsync().ConfigureAwait(false);
                try
                {
                    CreateDirectoryIfNotExists(fullFileName);
                    var oldFile = SaveBackup(fullFileName);
                    try
                    {
                        using (var stream = File.OpenWrite(fullFileName))
                        {
                            await ms.CopyToAsync(stream)
                                    .ConfigureAwait(false);
                        }

                        File.Delete(oldFile);
                    }
                    catch (Exception)
                    {
                        RestoreBackup(fullFileName, oldFile);
                        throw;
                    }
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
            }
        }

        internal static void Save<T>(T o, string fullFileName, Func<T, Stream> toStream)
        {
            SemaphoreSlim.Wait();
            CreateDirectoryIfNotExists(fullFileName);
            var oldFile = SaveBackup(fullFileName);
            try
            {
                var ms = toStream(o);
                using (var stream = File.OpenWrite(fullFileName))
                {
                    ms.CopyTo(stream);
                }
                File.Delete(oldFile);
            }
            catch (Exception)
            {
                RestoreBackup(fullFileName, oldFile);
                throw;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Creates the directory if not exists
        /// </summary>
        /// <param name="fullFileName"></param>
        internal static void CreateDirectoryIfNotExists(string fullFileName)
        {
            var directoryName = Path.GetDirectoryName(fullFileName);
            // ReSharper disable AssignNullToNotNullAttribute
            // Better to get system error message if null?
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        internal static string SaveBackup(string fullFileName, string oldExtension = ".old")
        {
            if (string.IsNullOrEmpty(fullFileName))
            {
                throw new ArgumentException("Filename cannot be null", "fullFileName");
            }
            if (string.IsNullOrEmpty(oldExtension))
            {
                throw new ArgumentException("Extension cannot be null","oldExtension");
            }
            var extension = System.IO.Path.GetExtension(fullFileName);
            var oldFile = fullFileName.Replace(extension, oldExtension);

            if (File.Exists(fullFileName))
            {
                if (File.Exists(oldFile))
                {
                    File.Delete(oldFile);
                }
                File.Move(fullFileName, oldFile);
            }
            return oldFile;
        }

        internal static void RestoreBackup(string fullFileName, string oldFile)
        {
            File.Delete(fullFileName);
            File.Move(oldFile, fullFileName);
        }
    }
}