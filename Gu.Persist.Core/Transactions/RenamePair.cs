namespace Gu.Persist.Core
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    [DebuggerDisplay("{Current} {Renamed}")]
    public class RenamePair : RenamePair<FileInfo>
    {
        public static readonly IReadOnlyList<RenamePair> EmptyArray = new RenamePair[0];

        public RenamePair(FileInfo current, FileInfo renamed)
            : base(current, renamed)
        {
        }
    }
}