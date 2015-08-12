namespace Gu.Settings.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class FileHelper
    {
        public static readonly string SoftDeleteExtension = ".delete";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="fromStream">Reading from from file to T</param>
        /// <returns></returns>
        internal static T Read<T>(this FileInfo file, Func<Stream, T> fromStream)
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
        internal static async Task<T> ReadAsync<T>(this FileInfo file, Func<Stream, T> fromStream)
        {
            using (var ms = await ReadAsync(file).ConfigureAwait(false))
            {
                return fromStream(ms);
            }
        }

        internal static async Task<MemoryStream> ReadAsync(this FileInfo file)
        {
            var ms = new MemoryStream();
            using (var fileStream = File.OpenRead(file.FullName))
            {
                await fileStream.CopyToAsync(ms)
                                .ConfigureAwait(false);
            }

            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Generic method for saving a file async
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal static async Task SaveAsync(this FileInfo file, Stream stream)
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
        internal static void Save(this FileInfo file, Stream stream)
        {
            using (var fileStream = File.OpenWrite(file.FullName))
            {
                stream.CopyTo(fileStream);
            }
        }

        internal static void HardDelete(this FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            file.DeleteSoftDeleteFileFor();
            file.Delete();
        }

        internal static void DeleteSoftDeleteFileFor(this FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            var softDelete = file.GetSoftDeleteFileFor();
            if (softDelete != null)
            {
                softDelete.Delete();
            }
        }

        internal static FileInfo SoftDelete(this FileInfo file)
        {
            file.Refresh();
            if (!file.Exists)
            {
                return null;
            }
            var soft = file.WithAppendedExtension(SoftDeleteExtension);
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

        internal static void SetIsVisible(this FileInfo file, bool isVisible)
        {
            Ensure.Exists(file, nameof(file));
            if (isVisible)
            {
                file.Attributes |= FileAttributes.Hidden;
            }
            else
            {
                file.Attributes &= ~FileAttributes.Hidden;
            }
        }

        internal static void Rename(this FileInfo oldName, FileInfo newName, bool owerWrite)
        {
            Ensure.Exists(oldName);
            if (!owerWrite)
            {
                Ensure.DoesNotExist(newName);
            }
            if (owerWrite)
            {
                newName.Delete();
            }
            oldName.MoveTo(newName);
        }
        internal static void MoveTo(this FileInfo source, FileInfo destination)
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

            backup.DeleteSoftDeleteFileFor();
            backup.SoftDelete();
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
            Ensure.NotNull(fileName, nameof(fileName));
            var file = CreateFileInfo(settings.Directory, fileName, settings.Extension);
            return file;
        }

        internal static FileInfo CreateFileInfo(DirectoryInfo directory, string fileName, string extension)
        {
            Ensure.NotNullOrEmpty(fileName, nameof(fileName));
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