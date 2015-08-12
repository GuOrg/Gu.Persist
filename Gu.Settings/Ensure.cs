namespace Gu.Settings
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class Ensure
    {
        internal static void NotNull(object o, string parameterName, [CallerMemberName] string caller = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (o == null)
            {
                var message = $"Expected parameter {parameterName} in member {caller} to not be null";
                throw new ArgumentNullException(parameterName, message);
            }
        }

        internal static void NotNull(object o, string parameterName, string message, [CallerMemberName] string caller = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (o == null)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(parameterName);
                }
                throw new ArgumentNullException(parameterName, message);
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

        public static void NotEqual<T>(T value, T other, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (Equals(value, other))
            {
                var message = $"Expected {value} to not equal {other}";
                throw new ArgumentException(message, parameterName);
            }
        }

        internal static void IsValidFileName(string s, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (!FileInfoExt.IsValidFileName(s))
            {
                var illegalCahrs = FileInfoExt.InvalidFileNameChars.Where(c => s.IndexOf(c) != -1).ToArray();
                var illegals = string.Join(", ", illegalCahrs.Select(x => string.Format("'{0}'", x)));
                var message = string.Format(@"{0} is not a valid filename. Contains: {{{1}}}", s, illegals);
                IsValidFileName(s, parameterName, message);
            }
        }

        internal static void IsValidFileName(string s, string parameterName, string message)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (!FileInfoExt.IsValidFileName(s))
            {
                throw new ArgumentException(parameterName, message);
            }
        }

        internal static void HasExtension(FileInfo file, string extension, string parameterName, string message = null)
        {
            NotNull(file, "file");
            NotNullOrEmpty(extension, "extension");
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (!string.Equals(file.Extension, extension, StringComparison.OrdinalIgnoreCase))
            {
                if (message == null)
                {
                    throw new ArgumentException(string.Format("Expected extension: {0}, was: {1}", extension, file.Extension), parameterName);
                }
                throw new ArgumentNullException(parameterName, message);
            }
        }

        internal static void ExtensionIsNot(FileInfo file, string extension, string parameterName, string message = null)
        {
            NotNull(file, "file");
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (string.Equals(file.Extension, extension, StringComparison.OrdinalIgnoreCase))
            {
                if (message == null)
                {
                    throw new ArgumentException(string.Format("Expected extension to not be {0}", extension), parameterName);
                }
                throw new ArgumentNullException(parameterName, message);
            }
        }

        internal static void ExtensionIsNotAnyOf(FileInfo file, string[] extensions, string parameterName, string message = null)
        {
            NotNull(file, "file");
            NotNull(extensions, "extensions");
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (extensions.Any(x => string.Equals(file.Extension, x, StringComparison.OrdinalIgnoreCase)))
            {
                if (message == null)
                {
                    throw new ArgumentException(string.Format("Expected not be any of {{{0}}}, was: {1}", string.Join(", ", extensions), file.Extension), parameterName);
                }
                throw new ArgumentNullException(parameterName, message);
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
