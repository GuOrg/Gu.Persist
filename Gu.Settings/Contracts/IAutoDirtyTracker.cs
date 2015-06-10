namespace Gu.Settings
{
    using System.Collections.Generic;

    public interface IAutoDirtyTracker
    {
        bool IsDirty<T>(T item);
      
        bool IsDirty<T>(T item, IEqualityComparer<T> comparer);
    }
}