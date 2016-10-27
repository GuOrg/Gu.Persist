namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <inheritdoc/>
    public sealed class DirectoryInfoComparer : EqualityComparer<DirectoryInfo>
    {
        public new static readonly DirectoryInfoComparer Default = new DirectoryInfoComparer();
        private static readonly char[] BackSlash = { '\\' };
        private static readonly StringComparer OrdinalIgnoreCaseComparer = StringComparer.OrdinalIgnoreCase;

        private DirectoryInfoComparer()
        {
        }

        /// <inheritdoc/>
        public override bool Equals(DirectoryInfo x, DirectoryInfo y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return OrdinalIgnoreCaseComparer.Equals(x.FullName.TrimEnd(BackSlash), y.FullName.TrimEnd(BackSlash));
        }

        /// <inheritdoc/>
        public override int GetHashCode(DirectoryInfo obj)
        {
            Ensure.NotNull(obj, nameof(obj));
            return OrdinalIgnoreCaseComparer.GetHashCode(obj.FullName.TrimEnd(BackSlash));
        }
    }
}