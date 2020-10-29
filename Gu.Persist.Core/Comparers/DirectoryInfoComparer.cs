namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <inheritdoc/>
    public sealed class DirectoryInfoComparer : EqualityComparer<DirectoryInfo>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        public new static readonly DirectoryInfoComparer Default = new DirectoryInfoComparer();
        private static readonly StringComparer OrdinalIgnoreCaseComparer = StringComparer.OrdinalIgnoreCase;
        private static readonly char[] BackSlash = { '\\' };

        private DirectoryInfoComparer()
        {
        }

        /// <inheritdoc/>
        public override bool Equals(DirectoryInfo? x, DirectoryInfo? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (OrdinalIgnoreCaseComparer.Equals(x.FullName, y.FullName))
            {
                return true;
            }

            if (Math.Abs(x.FullName.Length - y.FullName.Length) != 1)
            {
                return false;
            }

            return OrdinalIgnoreCaseComparer.Equals(TrimBackslash(x.FullName), TrimBackslash(y.FullName));
        }

        /// <inheritdoc/>
        public override int GetHashCode(DirectoryInfo obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return OrdinalIgnoreCaseComparer.GetHashCode(TrimBackslash(obj.FullName));
        }

        private static string TrimBackslash(string text)
        {
            if (text[text.Length - 1] == '\\')
            {
                return text.TrimEnd(BackSlash);
            }

            return text;
        }
    }
}