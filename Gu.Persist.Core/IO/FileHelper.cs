#pragma warning disable 1573
namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal static class FileHelper
    {
        internal static readonly string SoftDeleteExtension = ".delete";

        /// <summary>
        /// Read the contents of <paramref name="file"/> and deserialize it into an instance of <typeparamref name="T"/>
        /// </summary>
        /// <param name="fromStream">Deserializer</param>
        internal static T Read<T>(this FileInfo file, Func<Stream, T> fromStream)
        {
            using (var stream = File.OpenRead(file.FullName))
            {
                return fromStream(stream);
            }
        }

        /// <summary>
        /// Read the contents of <paramref name="file"/> and deserialize it into an instance of <typeparamref name="T"/>
        /// </summary>
        /// <param name="fromStream">Deserializer</param>
        internal static async Task<T> ReadAsync<T>(this FileInfo file, Func<Stream, T> fromStream)
        {
            using (var stream = await ReadAsync(file).ConfigureAwait(false))
            {
                return fromStream(stream);
            }
        }

        internal static async Task<Stream> ReadAsync(this FileInfo file)
        {
            var stream = PooledMemoryStream.Borrow();
            using (var fileStream = File.OpenRead(file.FullName))
            {
                await fileStream.CopyToAsync(stream)
                                .ConfigureAwait(false);
            }

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Generic method for saving a file async
        /// Creates or overwrites <paramref name="file"/>
        /// </summary>
        internal static async Task SaveAsync(this FileInfo file, Stream stream)
        {
            using (var fileStream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await stream.CopyToAsync(fileStream)
                            .ConfigureAwait(false);
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
            softDelete?.Delete();
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
            try
            {
                File.Move(file.FullName, soft.FullName);
            }
            catch (Exception)
            {
                // Swallowing here, no way to know that the file has not been touched.
                return null;
            }

            return soft;
        }

        // ReSharper disable once UnusedMember.Global
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

        internal static void MoveTo(this FileInfo source, FileInfo destination, bool overWrite)
        {
            destination.Refresh();
            if (destination.Exists && overWrite)
            {
                File.Delete(destination.FullName);
            }

            File.Move(source.FullName, destination.FullName);
        }

        internal static FileInfo CreateFileInfo<T>(IFileSettings setting)
        {
            return CreateFileInfo(typeof(T).Name, setting);
        }

        internal static FileInfo CreateFileInfo<TFileSettings>(string fileName, TFileSettings settings)
            where TFileSettings : IFileSettings
        {
            var file = new FileInfo(Path.Combine(settings.Directory, fileName + settings.Extension));
            return file;
        }

        internal static void Backup(FileInfo file, FileInfo backup)
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
            File.Move(file.FullName, backup.FullName);
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
            File.Move(backup.FullName, file.FullName);
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

        internal static string PrependDotIfMissing(string extension)
        {
            if (string.IsNullOrEmpty(extension))
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