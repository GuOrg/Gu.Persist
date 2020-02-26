namespace Gu.Persist.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    internal static class Ensure
    {
        [Obsolete]
        internal static void NotNullOrEmpty(string text, string paramName, string message = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (message is null)
                {
                    throw new ArgumentNullException(paramName);
                }

                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void ExtensionIsNot(FileInfo file, string extension, string parameterName, string message = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "parameter name is missing");
            if (string.Equals(file.Extension, extension, StringComparison.OrdinalIgnoreCase))
            {
                if (message is null)
                {
                    throw new ArgumentException($"Expected extension to not be {extension}.", parameterName);
                }

                throw new ArgumentException(parameterName, message);
            }
        }

        internal static void ExtensionIsNotAnyOf(FileInfo file, string[] extensions, string parameterName, string message = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "parameter name is missing");
            if (extensions.Any(x => string.Equals(file.Extension, x, StringComparison.OrdinalIgnoreCase)))
            {
                if (message is null)
                {
                    throw new ArgumentException($"Expected not be any of {{{string.Join(", ", extensions)}}}, was: {file.Extension}.", parameterName);
                }

                throw new ArgumentException(parameterName, message);
            }
        }

        internal static void Exists(FileInfo file, string message = null)
        {
            file.Refresh();
            if (!file.Exists)
            {
                if (message is null)
                {
                    throw new FileNotFoundException($"Expected file {file.FullName} to exist.");
                }

                throw new FileNotFoundException(message);
            }
        }

        internal static void DoesNotExist(FileInfo file, string message = null)
        {
            file.Refresh();
            if (file.Exists)
            {
                if (message is null)
                {
                    throw new InvalidOperationException($"Expected file {file.FullName} to not exist.");
                }

                throw new InvalidOperationException(message);
            }
        }
    }
}
