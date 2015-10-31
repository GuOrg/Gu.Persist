namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class DirectoryInfoComparer : IEqualityComparer<DirectoryInfo>
    {
        private static readonly char[] TrimEnd = { '\\' };
        public static readonly DirectoryInfoComparer Default = new DirectoryInfoComparer();

        private DirectoryInfoComparer()
        {
        }

        public bool Equals(DirectoryInfo x, DirectoryInfo y)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(x.FullName.TrimEnd(TrimEnd), y.FullName.TrimEnd(TrimEnd));
        }

        public int GetHashCode(DirectoryInfo obj)
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.FullName.TrimEnd(TrimEnd));
        }
    }
}