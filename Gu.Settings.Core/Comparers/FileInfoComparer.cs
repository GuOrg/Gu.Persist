namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class FileInfoComparer : IEqualityComparer<FileInfo>
    {
        public static readonly FileInfoComparer Default = new FileInfoComparer();
        public bool Equals(FileInfo x, FileInfo y)
        {
            x.Refresh();
            y.Refresh();
            return string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(FileInfo obj)
        {
            obj.Refresh();
            return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.FullName);
        }
    }
}
