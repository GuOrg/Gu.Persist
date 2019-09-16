namespace Gu.Persist.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class Ensure
    {
        [JetBrains.Annotations.ContractAnnotation("halt <= value:null")]
        internal static void NotNull<T>(T value, string parameterName, [CallerMemberName] string caller = null)
            where T : class
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "parameter name is missing");
            if (value == null)
            {
                var message = $"Expected parameter {parameterName} in member {caller} to not be null";
                throw new ArgumentNullException(parameterName, message);
            }
        }

        [JetBrains.Annotations.ContractAnnotation("halt <= text:null")]
        internal static void NotNullOrEmpty(string text, string paramName, string message = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }

                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void IsValidFileName(string s, string parameterName, string message)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "parameter name is missing");
            if (!FileInfoExt.IsValidFileName(s))
            {
                throw new ArgumentException(parameterName, message);
            }
        }

        // ReSharper disable once UnusedMember.Global
        internal static void HasExtension(FileInfo file, string extension, string parameterName, string message = null)
        {
            NotNullOrEmpty(extension, nameof(extension));
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "parameter name is missing");
            if (!string.Equals(file.Extension, extension, StringComparison.OrdinalIgnoreCase))
            {
                if (message == null)
                {
                    throw new ArgumentException($"Expected extension: {extension}, was: {file.Extension}.", parameterName);
                }

                throw new ArgumentException(parameterName, message);
            }
        }

        internal static void ExtensionIsNot(FileInfo file, string extension, string parameterName, string message = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "parameter name is missing");
            if (string.Equals(file.Extension, extension, StringComparison.OrdinalIgnoreCase))
            {
                if (message == null)
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
                if (message == null)
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
                if (message == null)
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
                if (message == null)
                {
                    throw new InvalidOperationException($"Expected file {file.FullName} to not exist.");
                }

                throw new InvalidOperationException(message);
            }
        }
    }
}
