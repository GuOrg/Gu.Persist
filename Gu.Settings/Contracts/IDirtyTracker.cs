namespace Gu.Settings
{
    using System.Collections.Generic;
    using System.IO;

    public interface IDirtyTracker : IAutoDirtyTracker
    {
        bool IsDirty<T>(T item, string fileName);

        bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer);

        bool IsDirty<T>(T item, FileInfo file);

        bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer);
    }
}