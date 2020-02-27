#pragma warning disable 1573
#pragma warning disable SA1600
namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal static class FileHelper
    {
#pragma warning disable CA1802 // Use literals where appropriate
        internal static readonly string SoftDeleteExtension = ".delete";
#pragma warning restore CA1802 // Use literals where appropriate

        /// <summary>
        /// Read the contents of <paramref name="file"/> and deserialize it into an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <typeparam name="TSettings">The type of <paramref name="setting"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="setting">The <typeparamref name="TSettings"/>.</param>
        /// <param name="serialize">Deserializer.</param>
        internal static T Read<T, TSettings>(this FileInfo file, TSettings setting, Serialize<TSettings> serialize)
        {
            using var stream = File.OpenRead(file.FullName);
            return serialize.FromStream<T>(stream, setting);
        }

        internal static Task<Stream> ReadAsync(this FileInfo file)
        {
            return ReadAsync(file.FullName);
        }

        internal static async Task<Stream> ReadAsync(this string fileName)
        {
            var stream = PooledMemoryStream.Borrow();
            using (var fileStream = File.OpenRead(fileName))
            {
                await fileStream.CopyToAsync(stream)
                                .ConfigureAwait(false);
            }

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Generic method for saving a file async
        /// Creates or overwrites <paramref name="file"/>.
        /// </summary>
        internal static async Task SaveAsync(this FileInfo file, Stream stream)
        {
            using var fileStream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream)
.ConfigureAwait(false);
        }

        internal static void HardDelete(this FileInfo file)
        {
            file.DeleteSoftDeleteFileFor();
            file.Delete();
        }

        internal static void DeleteSoftDeleteFileFor(this FileInfo file)
        {
            var softDelete = file.SoftDeleteFile();
            softDelete?.Delete();
        }

        internal static FileInfo? SoftDelete(this FileInfo file)
        {
            file.Refresh();
            if (!file.Exists)
            {
                return null;
            }

            var soft = file.WithAppendedExtension(SoftDeleteExtension);
            try
            {
                _ = Kernel32.MoveFileEx(file.FullName, soft.FullName, MoveFileFlags.REPLACE_EXISTING);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
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

        internal static FileInfo CreateFileInfo<T>(IFileSettings setting)
        {
            return CreateFileInfo(typeof(T).Name, setting);
        }

        internal static FileInfo CreateFileInfo<TFileSettings>(string fileName, TFileSettings settings)
            where TFileSettings : IFileSettings
        {
            var file = new FileInfo(Path.Combine(settings.Directory, fileName + (fileName.EndsWith(settings.Extension, StringComparison.InvariantCulture) ? string.Empty : settings.Extension)));
            return file;
        }

        internal static void Backup(FileInfo file, FileInfo backup)
        {
            if (backup is null)
            {
                return;
            }

            file.Refresh();
            if (!file.Exists)
            {
                return;
            }

            _ = backup.SoftDelete();
            _ = Kernel32.MoveFileEx(file.FullName, backup.FullName, MoveFileFlags.REPLACE_EXISTING);
            file.Refresh();
        }

        internal static void Restore(this FileInfo file, FileInfo backup)
        {
            if (backup is null)
            {
                return;
            }

            backup.Refresh();
            if (!backup.Exists)
            {
                return;
            }

            _ = Kernel32.MoveFileEx(backup.FullName, file.FullName, MoveFileFlags.REPLACE_EXISTING);
        }

        internal static FileInfo CreateFileInfo(DirectoryInfo? directory, string fileName, string extension)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (extension != null)
            {
                if (!extension.StartsWith(".", StringComparison.Ordinal))
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

            if (directory is null)
            {
                throw new ArgumentNullException("directory cannot be null when file name is not rooted.", nameof(directory));
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

            if (!extension.StartsWith(".", StringComparison.Ordinal))
            {
                return "." + extension;
            }

            return extension;
        }
    }
}