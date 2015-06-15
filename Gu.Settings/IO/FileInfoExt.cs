namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    public static class FileInfoExt
    {
        private static ConcurrentDictionary<string, string> TimeStampPatternMap = new ConcurrentDictionary<string, string>();

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

        internal static FileInfo AppendExtension(this FileInfo file, string extension)
        {
            Ensure.NotNullOrEmpty(extension, "extension");
            extension = FileHelper.PrependDotIfMissing(extension);
            return new FileInfo(string.Concat(file.FullName, extension));
        }

        internal static FileInfo RemoveExtension(this FileInfo file, string extension)
        {
            Ensure.NotNullOrEmpty(extension, "extension");
            var ext = Path.GetExtension(file.FullName);
            extension = FileHelper.PrependDotIfMissing(extension);
            if (ext != extension)
            {
                throw new ArgumentException("Fail", "extension");
            }
            string fileName = file.FullName.Substring(0, (file.FullName.Length - extension.Length));
            return new FileInfo(fileName);
        }

        internal static DateTime GetTimeStamp(this FileInfo file, IBackupSettings setting)
        {
            Ensure.NotNull(setting, "setting");
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

        internal static FileInfo AddTimeStamp(this FileInfo file, DateTime time, IBackupSettings setting)
        {
            if (setting == null)
            {
                return file;
            }
            if (String.IsNullOrWhiteSpace(setting.TimeStampFormat) && setting.NumberOfBackups <= 1)
            {
                return file;
            }
            var timestamp = string.Format(".{0}{1}", time.ToString(setting.TimeStampFormat, CultureInfo.InvariantCulture), file.Extension);
            var timestamped = file.ChangeExtension(timestamp);
            return timestamped;
        }

        internal static FileInfo RemoveTimeStamp(this FileInfo file, IBackupSettings setting)
        {
            Ensure.NotNull(setting, "setting");
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
