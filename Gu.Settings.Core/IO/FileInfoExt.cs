namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    public static class FileInfoExt
    {
        private static ConcurrentDictionary<string, string> TimeStampPatternMap = new ConcurrentDictionary<string, string>();
        public static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        public static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

        public static bool IsValidFileName(string filename)
        {
            var indexOfAny = filename.IndexOfAny(InvalidFileNameChars);
            return indexOfAny == -1;
        }

        internal static bool CanRename(this FileInfo file, string newName, IFileSettings settings)
        {
            if (!file.Exists)
            {
                return false;
            }
            var withNewName = file.WithNewName(newName, settings);
            return !withNewName.Exists;
        }

        /// <summary>
        /// Changes extension and returns the new fileinfo.
        /// No IO
        /// </summary>
        /// <param name="file"></param>
        /// <param name="newExtension"></param>
        /// <returns></returns>
        internal static FileInfo WithNewExtension(this FileInfo file, string newExtension)
        {
            newExtension = FileHelper.PrependDotIfMissing(newExtension);
            var newFileName = Path.ChangeExtension(file.FullName, newExtension);
            var newFile = new FileInfo(newFileName);
            return newFile;
        }

        internal static FileInfo InDirectory(this FileInfo file, DirectoryInfo directory)
        {
            if (file.Directory == directory)
            {
                return file;
            }
            return directory.CreateFileInfoInDirectory(file.Name);
        }

        internal static FileInfo GetSoftDeleteFileFor(this FileInfo file)
        {
            return file.WithAppendedExtension(FileHelper.SoftDeleteExtension);
        }

        internal static bool GetIsSoftDeleteFile(this FileInfo file)
        {
            return file.FullName.EndsWith(FileHelper.SoftDeleteExtension, StringComparison.OrdinalIgnoreCase);
        }

        internal static FileInfo WithAppendedExtension(this FileInfo file, string extension)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNullOrEmpty(extension, nameof(extension));
            extension = FileHelper.PrependDotIfMissing(extension);
            return new FileInfo(string.Concat(file.FullName, extension));
        }

        internal static FileInfo WithRemovedExtension(this FileInfo file, string extension)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNullOrEmpty(extension, nameof(extension));
            var ext = Path.GetExtension(file.FullName);
            extension = FileHelper.PrependDotIfMissing(extension);
            if (ext != extension)
            {
                throw new ArgumentException("Fail", "extension");
            }
            string fileName = file.FullName.Substring(0, (file.FullName.Length - extension.Length));
            return new FileInfo(fileName);
        }

        internal static FileInfo WithNewName(this FileInfo file, string newName, IFileSettings setting)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.IsValidFileName(newName, nameof(newName));
            Ensure.NotNull(setting, nameof(setting));
            FileInfo newFile;
            var isSoftDeleteFile = file.GetIsSoftDeleteFile();
            if (isSoftDeleteFile)
            {
                if (newName.EndsWith(FileHelper.SoftDeleteExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    newName = newName.Substring(0, (newName.Length - FileHelper.SoftDeleteExtension.Length));
                }
                file = file.WithRemovedExtension(FileHelper.SoftDeleteExtension);
            }
            if (Path.HasExtension(newName))
            {
                newFile = setting.Directory.CreateFileInfoInDirectory(newName);
            }
            else
            {
                newFile = setting.Directory.CreateFileInfoInDirectory(String.Concat(newName, file.Extension));
            }
            var backupSettings = setting as IBackupSettings;
            if (backupSettings != null)
            {
                var timeStamp = file.GetTimeStamp(backupSettings);
                newFile = newFile.WithTimeStamp(timeStamp, backupSettings);
            }
            if (isSoftDeleteFile && !newFile.GetIsSoftDeleteFile())
            {
                newFile = newFile.WithAppendedExtension(FileHelper.SoftDeleteExtension);
            }
            return newFile;
        }

        internal static string GetFileNameWithoutExtension(this FileInfo file)
        {
            return Path.GetFileNameWithoutExtension(file.Name);
        }

        internal static DateTime GetTimeStamp(this FileInfo file, IBackupSettings setting)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(setting, nameof(setting));
            if (setting.TimeStampFormat == null)
            {
                return file.CreationTime;
            }
            var pattern = setting.TimeStampPattern();
            var timeAsString = Regex.Match(file.FullName, pattern, RegexOptions.RightToLeft | RegexOptions.Singleline)
                                    .Groups["timestamp"]
                                    .Value;
            if (string.IsNullOrEmpty(timeAsString))
            {
                return file.CreationTime;
            }
            var timeStamp = DateTime.ParseExact(timeAsString, setting.TimeStampFormat, CultureInfo.InvariantCulture);
            return timeStamp;
        }

        internal static FileInfo WithTimeStamp(this FileInfo file, DateTime time, IBackupSettings setting)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(setting, nameof(setting));

            if (String.IsNullOrWhiteSpace(setting.TimeStampFormat) && setting.NumberOfBackups <= 1)
            {
                return file;
            }
            var timestamp = string.Format(".{0}{1}", time.ToString(setting.TimeStampFormat, CultureInfo.InvariantCulture), file.Extension);
            var timestamped = file.WithNewExtension(timestamp);
            return timestamped;
        }

        internal static FileInfo WithRemovedTimeStamp(this FileInfo file, IBackupSettings setting)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(setting, nameof(setting));
            if (setting.TimeStampFormat == null)
            {
                return file;
            }
            var pattern = setting.TimeStampPattern();
            var stripped = Regex.Replace(file.FullName, pattern, "", RegexOptions.RightToLeft | RegexOptions.Singleline);
            return new FileInfo(stripped);
        }

        internal static string TimeStampPattern(this IBackupSettings setting)
        {
            if (setting == null || string.IsNullOrEmpty(setting.TimeStampFormat))
            {
                return "";
            }
            var format = setting.TimeStampFormat;
            var pattern = TimeStampPatternMap.GetOrAdd(format, CreateTimeStampPattern);
            return pattern;
        }

        // Internal for tests
        internal static string CreateTimeStampPattern(string format)
        {
            var pattern = Regex.Escape(format);
            pattern = Regex.Replace(pattern, "(d+)", @"\d+", RegexOptions.IgnoreCase);
            pattern = Regex.Replace(pattern, "(y+)", @"\d+", RegexOptions.IgnoreCase);
            pattern = Regex.Replace(pattern, "(m+)", @"\d+", RegexOptions.IgnoreCase);
            pattern = Regex.Replace(pattern, "(h+)", @"\d+", RegexOptions.IgnoreCase);
            pattern = Regex.Replace(pattern, "(s+)", @"\d+", RegexOptions.IgnoreCase);
            return string.Format(@"\.(?<timestamp>{0})", pattern);
        }
    }
}
