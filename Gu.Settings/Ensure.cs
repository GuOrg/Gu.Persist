namespace Gu.Settings
{
    using System;
    using System.IO;
    using System.Linq;

    internal static class Ensure
    {
        internal static void NotNull(object o, string paramName, string message = null)
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

        internal static void Extension(FileInfo file, string extension, string paramName, string message = null)
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
