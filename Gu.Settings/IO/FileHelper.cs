namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;

    public static class FileHelper
    {
        public static readonly string SoftDeleteExtension = ".delete";
        private static readonly ConcurrentDictionary<string, FileInfo> FileNamesMap = new ConcurrentDictionary<string, FileInfo>(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<FileInfo, IFileInfos> FileInfosMap = new ConcurrentDictionary<FileInfo, IFileInfos>(FileInfoComparer.Default);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="fromStream">Reading from from file to T</param>
        /// <returns></returns>
        public static T Read<T>(FileInfo file, Func<Stream, T> fromStream)
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
        public static async Task<T> ReadAsync<T>(FileInfo file, Func<Stream, T> fromStream)
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
        public static async Task SaveAsync(FileInfo file, Stream stream)
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
        public static void Save(FileInfo file, Stream stream)
        {
            using (var fileStream = File.OpenWrite(file.FullName))
            {
                stream.CopyTo(fileStream);
            }
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

            backup.HardDelete();
            file.MoveTo(backup);
            file.Refresh();
        }

        internal static void Restore(FileInfo file, FileInfo backup)
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
            FileInfo fileInfo;
            if (FileNamesMap.TryGetValue(fileName, out fileInfo))
            {
                return fileInfo;
            }
            fileInfo = CreateFileInfo(settings.Directory, fileName, settings.Extension);
            FileNamesMap.TryAdd(fileName, fileInfo);
            return fileInfo;
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

        public static FileInfo ChangeExtension(this FileInfo file, string newExtension)
        {
            Ensure.NotNullOrEmpty(newExtension, "newExtension");
            if (!newExtension.StartsWith("."))
            {
                newExtension = "." + newExtension;
            }
            var newFileName = Path.ChangeExtension(file.FullName, newExtension);
            var newFile = new FileInfo(newFileName);
            return newFile;
        }

        internal static void HardDelete(this FileInfo file)
        {
            var soft = string.Concat(file.FullName, SoftDeleteExtension);
            File.Delete(soft);
            file.Delete();
        }

        internal static FileInfo SoftDelete(this FileInfo file)
        {
            var soft = string.Concat(file.FullName, SoftDeleteExtension);
            File.Delete(soft);
            File.Move(file.Name, soft);
            return new FileInfo(soft);
        }

        [Obsolete("Refactor away from fileinfos")]
        internal static IFileInfos GetFileInfos(FileInfo file, RepositorySetting setting)
        {
            if (setting.BackupSettings.CreateBackups)
            {
                return FileInfosMap.GetOrAdd(
                    file,
                    x => FileInfos.CreateFileInfos(file, setting.TempExtension, setting.BackupSettings.Extension));
            }
            return FileInfosMap.GetOrAdd(file, x => FileInfos.CreateFileInfos(file, setting.TempExtension, null));
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