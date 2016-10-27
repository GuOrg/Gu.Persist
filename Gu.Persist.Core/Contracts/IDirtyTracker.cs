namespace Gu.Persist.Core
{
    using System.Collections.Generic;

    public interface IDirtyTracker
    {
        bool IsDirty<T>(T item, string fullFileName, IEqualityComparer<T> comparer);

        void Track<T>(string fullFileName, T item);

        void Rename(string oldName, string newName, bool owerWrite);

        void ClearCache();

        void RemoveFromCache(string fullFileName);
    }
}