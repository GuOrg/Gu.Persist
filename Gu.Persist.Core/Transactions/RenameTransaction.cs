#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal sealed class RenameTransaction : IDisposable
    {
        private readonly IReadOnlyList<RenamePair> pairs;
        private readonly List<RenamePair<LockedFile>> lockedPairs = new List<RenamePair<LockedFile>>();

        internal RenameTransaction(IReadOnlyList<RenamePair> pairs)
        {
            this.pairs = pairs;
        }

        public void Dispose()
        {
            foreach (var lockedFile in this.lockedPairs)
            {
                lockedFile.Current?.Dispose();
                lockedFile.Renamed?.Dispose();
            }
        }

        internal void Commit(bool overWrite)
        {
            foreach (var pair in this.pairs.Distinct(RenamePairComparer.Default))
            {
#pragma warning disable CA2000, IDE0068 // Dispose objects before losing scope
                var current = LockedFile.Create(pair.Current, f => f.Open(FileMode.Open, FileAccess.Read, FileShare.Delete));
#pragma warning restore CA2000, IDE0068 // Dispose objects before losing scope
                if (!overWrite && pair.Renamed.Exists)
                {
                    current.Dispose();
                    var message = $"Renaming:\r\n" + $"Current: {pair.Current}\r\n" + $"To:      {pair.Renamed}\r\n" +
                                  $"Would overwrite destination.\r\n" + $" If you mean to overwrite pass in true.";
                    throw new InvalidOperationException(message);
                }

                try
                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                    var renamed = LockedFile.Create(pair.Renamed, f => f.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Delete));
#pragma warning restore CA2000 // Dispose objects before losing scope
                    this.lockedPairs.Add(new RenamePair<LockedFile>(current, renamed));
                }
                catch
                {
                    current?.Dispose();
                    throw;
                }
            }

            foreach (var pair in this.lockedPairs)
            {
                pair.Renamed.DisposeAndDeleteFile();
                _ = Kernel32.MoveFileEx(pair.Current.File.FullName, pair.Renamed.File.FullName, MoveFileFlags.REPLACE_EXISTING);
            }
        }

        private class RenamePairComparer : IEqualityComparer<RenamePair>
        {
            internal static readonly RenamePairComparer Default = new RenamePairComparer();

            private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

            private RenamePairComparer()
            {
            }

            public bool Equals(RenamePair x, RenamePair y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return StringComparer.Equals(x.Current.FullName, y.Current.FullName);
            }

            public int GetHashCode(RenamePair obj)
            {
                if (obj is null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }

                return StringComparer.GetHashCode(obj.Current.FullName) ^
                       StringComparer.GetHashCode(obj.Renamed.FullName);
            }
        }
    }
}