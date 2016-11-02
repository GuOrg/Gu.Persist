namespace Gu.Persist.Core
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// The files before and after a rename operation.
    /// </summary>
    [DebuggerDisplay("{Current} {Renamed}")]
    public class RenamePair : RenamePair<FileInfo>
    {
        /// <summary>
        /// Gets an empty array of <see cref="RenamePair"/>
        /// </summary>
        public static readonly IReadOnlyList<RenamePair> EmptyArray = new RenamePair[0];

        /// <summary> Initializes a new instance of the <see cref="RenamePair"/> class. </summary>
        public RenamePair(FileInfo current, FileInfo renamed)
            : base(current, renamed)
        {
        }
    }
}