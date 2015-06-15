namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    public static class FileHelper
    {
        public static readonly string SoftDeleteExtension = ".delete";
        //private static readonly ConcurrentDictionary<string, FileInfo> FileNamesMap = new ConcurrentDictionary<string, FileInfo>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="fromStream">Reading from from file to T</param>
        /// <returns></returns>
        public static T Read<T>(this FileInfo file, Func<Stream, T> fromStream)
        {
            using (var stream = File.OpenRead(file.FullName))
            {
                return fromStream(stream);
            }
        }

        /// <summary>
        /// Reads the contents of the file to a memorystream
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fromStream">Reading from stream to T</param>
        /// <returns></returns>
        public static async Task<T> ReadAsync<T>(this FileInfo file, Func<Stream, T> fromStream)
        {
            using (var ms = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(file.FullName))
                {
                    await fileStream.CopyToAsync(ms)
                                    .ConfigureAwait(false);
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
        public static async Task SaveAsync(this FileInfo file, Stream stream)
        {
            using (var fileStream = file.OpenWrite())
            {
                await stream.CopyToAsync(fileStream)
                            .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        public static void Save(this FileInfo file, Stream stream)
        {
            using (var fileStream = File.OpenWrite(file.FullName))
            {
                stream.CopyTo(fileStream);
            }
        }

        internal static void HardDelete(this FileInfo file)
        {
            var soft = string.Concat(file.FullName, SoftDeleteExtension);
            File.Delete(soft);
            file.Delete();
        }

        internal static FileInfo SoftDelete(this FileInfo file)
        {
            file.Refresh();
            if (!file.Exists)
            {
                return null;
            }
            var soft = file.AppendExtension(SoftDeleteExtension);
            File.Delete(soft.FullName);
            try // Swallowing here, no way to know that the file has not been touched.
            {
                File.Move(file.FullName, soft.FullName); 
            }
            catch (Exception)
            {
                return null;
            }

            return soft;
        }

        public static void MoveTo(this FileInfo source, FileInfo destination)
        {
            destination.Refresh();
            if (destination.Exists)
            {
                File.Delete(destination.FullName);
            }
            File.Move(source.FullName, destination.FullName);
        }

        internal static void Backup(this FileInfo file, FileInfo backup)
        {
            if (backup == null)
            {
                return;
            }

            file.Refresh();
            if (!file.Exists)
            {
                return;
            }

            backup.HardDelete();
            file.MoveTo(backup);
            file.Refresh();
        }

        internal static void Restore(this FileInfo file, FileInfo backup)
        {
            if (backup == null)
            {
                return;
            }
            backup.Refresh();
            if (!backup.Exists)
            {
                return;
            }
            file.Delete();
            backup.MoveTo(file);
        }

        public static FileInfo CreateFileInfo<T>(IFileSettings setting)
        {
            return CreateFileInfo(typeof(T).Name, setting);
        }

        public static FileInfo CreateFileInfo(string fileName, IFileSettings settings)
        {
            Ensure.NotNull(fileName, "fileName");
            var file = CreateFileInfo(settings.Directory, fileName, settings.Extension);
            return file;
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
                    if (!String.Equals(ext, extension, StringComparison.OrdinalIgnoreCase))
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

        internal static string PrependDotIfMissing(string extension)
        {
            if (String.IsNullOrEmpty(extension))
            {
                return extension;
            }
            if (!extension.StartsWith("."))
            {
                return "." + extension;
            }
            return extension;
        }
    }
}