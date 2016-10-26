namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public sealed class FileInfoComparer : EqualityComparer<FileInfo>
    {
        public new static readonly FileInfoComparer Default = new FileInfoComparer();

        public override bool Equals(FileInfo x, FileInfo y)
        {
            x.Refresh();
            y.Refresh();
            return string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(FileInfo obj)
        {
            obj.Refresh();
            return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.FullName);
        }
    }
}
