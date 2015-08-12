namespace Gu.Settings
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public sealed class CollectionItemTrackerCollection : ChangeTracker, IReadOnlyCollection<IValueTracker>
    {
        private readonly Type _parentType;
        private readonly PropertyInfo _parentProperty;
        private readonly ChangeTrackerSettings _settings;
        private readonly List<IValueTracker> _trackers = new List<IValueTracker>();

        public CollectionItemTrackerCollection(Type parentType, PropertyInfo parentProperty, ChangeTrackerSettings settings)
        {
            Ensure.NotNull(parentType, nameof(parentType));
            Ensure.NotNull(parentProperty, nameof(parentProperty));
            _parentType = parentType;
            _parentProperty = parentProperty;
            _settings = settings;
        }

        public int Count { get { return _trackers.Count; } }

        public bool IsReadOnly { get { return true; } }

        public IEnumerator<IValueTracker> GetEnumerator()
        {
            return _trackers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(IValueTracker item)
        {
            VerifyDisposed();
            return _trackers.Contains(item);
        }

        internal void Add(IEnumerable items)
        {
            foreach (var child in items)
            {
                var itemTracker = Create(_parentType, _parentProperty, child, _settings);
                if (itemTracker != null)
                {
                    Add(itemTracker);
                }
            }
        }

        internal void Clear()
        {
            VerifyDisposed();
            ClearCore();
        }

        /// <summary>
        /// Make the class sealed when using this. 
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearCore();
            }
        }

        private void ClearCore()
        {
            foreach (var tracker in _trackers)
            {
                if (tracker != null)
                {
                    tracker.Dispose();
                    tracker.PropertyChanged -= OnItemPropertyChanged;
                }
            }
            _trackers.Clear();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != ChangesPropertyName)
            {
                return;
            }
            Changes++;
        }

        private void Add(IValueTracker item)
        {
            var match = _trackers.FirstOrDefault(x => ReferenceEquals(x.Value, item));
            if (match != null)
            {
                throw new InvalidOperationException("Cannot track the same item twice. Clear before add");
            }
            item.PropertyChanged += OnItemPropertyChanged;
            _trackers.Add(item);
        }

        private bool Remove(IValueTracker item)
        {
            VerifyDisposed();
            var remove = _trackers.Remove(item);
            if (remove)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
                item.Dispose();
            }
            return remove;
        }

        private bool RemoveBy(object item)
        {
            VerifyDisposed();
            var match = _trackers.FirstOrDefault(x => ReferenceEquals(x.Value, item));
            if (match != null)
            {
                return Remove(match);
            }
            return false;
        }
    }
}
