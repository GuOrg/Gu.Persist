namespace Gu.Settings
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    using Gu.Settings.Internals;

    public sealed class CollectionTracker : PropertyTracker
    {
        private readonly ChangeTrackerSettings _settings;
        private readonly CollectionItemTrackerCollection _itemTrackers;

        public CollectionTracker(Type parentType, PropertyInfo parentProperty, IEnumerable value, ChangeTrackerSettings settings)
            : base(parentType, parentProperty, value)
        {
            _settings = settings;
            _itemTrackers = new CollectionItemTrackerCollection(parentType, parentProperty, settings);
            _itemTrackers.PropertyChanged += OnSubtrackerPropertyChanged;
            var incc = value as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged += OnItemsChanged;
            }
            _itemTrackers.Add(value);
        }

        private IEnumerable Items
        {
            get { return (IEnumerable)base.Value; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var incc = Value as INotifyCollectionChanged;
                if (incc != null)
                {
                    incc.CollectionChanged -= OnItemsChanged;
                }

                _itemTrackers.Dispose();
                _itemTrackers.PropertyChanged -= OnSubtrackerPropertyChanged;
            }
            base.Dispose(disposing);
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Changes++;
            var type = sender.GetType();
            if (type.IsEnumerableOfT())
            {
                var itemType = type.GetItemType();
                if (itemType != null && !IsTrackType(itemType, _settings))
                {
                    return;
                }
            }
            _itemTrackers.Clear(); // keeping it simple here.
            _itemTrackers.Add((IEnumerable)sender);
        }

        private void OnSubtrackerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Changes))
            {
                Changes++;
            }
        }
    }
}