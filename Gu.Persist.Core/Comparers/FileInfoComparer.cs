namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <inheritdoc/>
    public sealed class FileInfoComparer : EqualityComparer<FileInfo>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        public new static readonly FileInfoComparer Default = new FileInfoComparer();

        /// <inheritdoc/>
        public override bool Equals(FileInfo x, FileInfo y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            x.Refresh();
            y.Refresh();
            return string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override int GetHashCode(FileInfo obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.Refresh();
            return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.FullName);
        }
    }
}
