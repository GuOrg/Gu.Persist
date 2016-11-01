namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public sealed class RenameTransaction : IDisposable
    {
        private readonly IReadOnlyList<RenamePair> pairs;
        private readonly List<RenamePair<LockedFile>> lockedPairs = new List<RenamePair<LockedFile>>();

        public RenameTransaction(IReadOnlyList<RenamePair> pairs)
        {
            this.pairs = pairs;
        }

        public void Commit(bool overWrite)
        {
            foreach (var pair in this.pairs.Distinct(RenamePairComparer.Default))
            {
                var current = LockedFile.Create(pair.Current, f => f.Open(FileMode.Open, FileAccess.Read, FileShare.Delete));
                if (!overWrite && pair.Renamed.Exists)
                {
                    current.Dispose();
                    var message = $"Renaming:\r\n" + $"Current: {pair.Current}\r\n" + $"To:      {pair.Renamed}\r\n" +
                                  $"Would overwrite destination.\r\n" + $" If you mean to overwrite pass in true.";
                    throw new InvalidOperationException(message);
                }

                try
                {
                    var renamed = LockedFile.Create(pair.Renamed, f => f.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Delete));
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
                File.Move(pair.Current.File.FullName, pair.Renamed.File.FullName);
            }
        }

        public void Dispose()
        {
            foreach (var lockedFile in this.lockedPairs)
            {
                lockedFile.Current?.Dispose();
                lockedFile.Renamed?.Dispose();
            }
        }

        private class RenamePairComparer : IEqualityComparer<RenamePair>
        {
            private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

            public static readonly RenamePairComparer Default = new RenamePairComparer();

            private RenamePairComparer()
            {
            }

            public bool Equals(RenamePair x, RenamePair y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return StringComparer.Equals(x.Current.FullName, y.Current.FullName);
            }

            public int GetHashCode(RenamePair obj)
            {
                Ensure.NotNull(obj, nameof(obj));
                return StringComparer.GetHashCode(obj.Current.FullName) ^
                       StringComparer.GetHashCode(obj.Renamed.FullName);
            }
        }
    }
}