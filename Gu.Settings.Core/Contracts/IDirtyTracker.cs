namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IDirtyTracker : IDisposable
    {
        bool IsDirty<T>(T item, FileInfo file, IEqualityComparer<T> comparer);

        void Track<T>(FileInfo file, T item);

        void Rename(FileInfo oldName, FileInfo newName, bool owerWrite);
      
        void ClearCache();

        void RemoveFromCache(FileInfo file);
    }
}