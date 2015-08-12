namespace Gu.Settings
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    [DebuggerDisplay("Count: {Count}")]
    public sealed class PropertyTrackerCollection : ChangeTracker, IReadOnlyCollection<IPropertyTracker>, IDisposable
    {
        private readonly Type _parentType;
        private readonly ChangeTrackerSettings _settings;
        private readonly List<IPropertyTracker> _trackers = new List<IPropertyTracker>();

        public PropertyTrackerCollection(Type parentType, ChangeTrackerSettings settings)
        {
            _parentType = parentType;
            _settings = settings;
        }

        public int Count { get { return _trackers.Count; } }

        public bool IsReadOnly { get { return false; } }

        public IEnumerator<IPropertyTracker> GetEnumerator()
        {
            VerifyDisposed();
            return _trackers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void Add(INotifyPropertyChanged item, IReadOnlyList<PropertyInfo> trackProperties)
        {
            Ensure.NotNull(item, nameof(item));
            Ensure.NotNull(trackProperties, nameof(trackProperties));
            foreach (var property in trackProperties)
            {
                Add(item, property);
            }
        }

        internal void Add(INotifyPropertyChanged item, PropertyInfo property)
        {
            Ensure.NotNull(item, nameof(item));
            Ensure.NotNull(property, nameof(property));
            var value = property.GetValue(item);
            if (!CanTrack(_parentType, property, value, _settings))
            {
                return;
            }
            if (value == null)
            {
                return;
            }
            var tracker = Create(_parentType, property, value, _settings);
            if (tracker != null)
            {
                Add(tracker);
            }
        }

        internal void Clear()
        {
            VerifyDisposed();
            ClearCore();
        }

        internal void RemoveBy(PropertyInfo propertyInfo)
        {
            var toRemove = _trackers.SingleOrDefault(x => x.ParentProperty == propertyInfo);
            if (toRemove != null)
            {
                Remove(toRemove);
            }
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

        private void Add(IPropertyTracker tracker)
        {
            VerifyDisposed();
            if (tracker == null)
            {
                return;
            }
            tracker.PropertyChanged += OnItemPropertyChanged;
            var old = _trackers.SingleOrDefault(x => x.ParentProperty.Name == tracker.ParentProperty.Name);
            if (old != null)
            {
                var message =
                    string.Format(
                        "Cannot have two trackers for the same property: {0}.{1} of the same instance: {2}. Remove old before adding new",
                        tracker.ParentType.Name,
                        tracker.ParentProperty.Name,
                        tracker.Value);
                throw new InvalidOperationException(message);
            }

            _trackers.Add(tracker);
        }

        private bool Remove(IPropertyTracker item)
        {
            VerifyDisposed();
            var removed = _trackers.Remove(item);
            if (removed)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
                item.Dispose();
            }

            return removed;
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
    }
}