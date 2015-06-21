namespace Gu.Settings
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class Ensure
    {
        internal static void NotNull(object o, string paramName, [CallerMemberName] string caller = null)
        {
            NotNullOrEmpty(paramName, "paramName");
            if (o == null)
            {
                var message = string.Format("Expected parameter {0} in member {1} to not be null", paramName, caller);
                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void NotNull(object o, string paramName, string message, [CallerMemberName] string caller = null)
        {
            NotNullOrEmpty(paramName, "paramName");
            if (o == null)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void NotNullOrEmpty(string s, string paramName, string message = null)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        public static void NotEqual<T>(T value, T other, string parameter)
        {
            if (Equals(value, other))
            {
                var message = string.Format("Expected {0} to not equal {1}", value, other);
                throw new ArgumentException(message, parameter);
            }
        }

        internal static void IsValidFileName(string s, string paramName)
        {
            NotNullOrEmpty(paramName, "paramName");
            if (!FileInfoExt.IsValidFileName(s))
            {
                var illegalCahrs = FileInfoExt.InvalidFileNameChars.Where(c => s.IndexOf(c) != -1).ToArray();
                var illegals = string.Join(", ", illegalCahrs.Select(x => string.Format("'{0}'", x)));
                var message = string.Format(@"{0} is not a valid filename. Contains: {{{1}}}", s, illegals);
                IsValidFileName(s, paramName, message);
            }
        }

        internal static void IsValidFileName(string s, string paramName, string message)
        {
            NotNullOrEmpty(paramName, "paramName");
            if (!FileInfoExt.IsValidFileName(s))
            {
                throw new ArgumentException(paramName, message);
            }
        }

        internal static void HasExtension(FileInfo file, string extension, string paramName, string message = null)
        {
            NotNull(file, "file");
            NotNullOrEmpty(extension, "extension");
            NotNullOrEmpty(paramName, "paramName");
            if (!string.Equals(file.Extension, extension, StringComparison.OrdinalIgnoreCase))
            {
                if (message == null)
                {
                    throw new ArgumentException(string.Format("Expected extension: {0}, was: {1}", extension, file.Extension), paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void ExtensionIsNot(FileInfo file, string extension, string paramName, string message = null)
        {
            NotNull(file, "file");
            NotNullOrEmpty(paramName, "paramName");
            if (string.Equals(file.Extension, extension, StringComparison.OrdinalIgnoreCase))
            {
                if (message == null)
                {
                    throw new ArgumentException(string.Format("Expected extension to not be {0}", extension), paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void ExtensionIsNotAnyOf(FileInfo file, string[] extensions, string paramName, string message = null)
        {
            NotNull(file, "file");
            NotNull(extensions, "extensions");
            NotNullOrEmpty(paramName, "paramName");
            if (extensions.Any(x => string.Equals(file.Extension, x, StringComparison.OrdinalIgnoreCase)))
            {
                if (message == null)
                {
                    throw new ArgumentException(string.Format("Expected not be any of {{{0}}}, was: {1}", string.Join(", ", extensions), file.Extension), paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void Exists(FileInfo file, string message = null)
        {
            NotNull(file, "file");
            file.Refresh();
            if (!file.Exists)
            {
                if (message == null)
                {
                    throw new InvalidOperationException(string.Format("Expected file {0} to exist", file.FullName));
                }
                throw new InvalidOperationException(message);
            }
        }

        internal static void DoesNotExist(FileInfo file, string message = null)
        {
            NotNull(file, "file");
            file.Refresh();
            if (file.Exists)
            {
                if (message == null)
                {
                    throw new InvalidOperationException(string.Format("Expected file {0} to not exist", file.FullName));
                }
                throw new InvalidOperationException(message);
            }
        }
    }
}
