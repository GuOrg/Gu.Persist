namespace Gu.Settings
{
    using System.Collections.Generic;
    using System.IO;

    public interface IDirtyTracker
    {
        bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer);

        void TrackOrUpdate<T>(FileInfo file, T item);

        void Rename(FileInfo oldName, FileInfo newName, bool owerWrite);
      
        void ClearCache();

        void RemoveFromCache(FileInfo file);
    }
}