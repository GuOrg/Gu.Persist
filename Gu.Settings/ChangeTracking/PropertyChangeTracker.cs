namespace Gu.Settings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public sealed class PropertyChangeTracker : PropertyTracker
    {
        private readonly ChangeTrackerSettings _settings;
        private readonly PropertyTrackerCollection _propertyTrackers;

        internal PropertyChangeTracker(Type parentType, PropertyInfo parentProperty, INotifyPropertyChanged value, ChangeTrackerSettings settings)
            : base(parentType, parentProperty, value)
        {
            Ensure.NotNull(parentType, nameof(parentType));
            Ensure.NotNull(parentProperty, nameof(parentProperty));
            Ensure.NotNull(value, nameof(value));
            Ensure.NotNull(settings, nameof(settings));

            _settings = settings;
            _propertyTrackers = new PropertyTrackerCollection(value.GetType(), settings);
            value.PropertyChanged += OnItemPropertyChanged;
            _propertyTrackers.PropertyChanged += OnSubtrackerPropertyChanged;
            _propertyTrackers.Add(value, TrackProperties);
        }

        private new INotifyPropertyChanged Value
        {
            get { return (INotifyPropertyChanged)base.Value; }
        }

        public IReadOnlyList<PropertyInfo> TrackProperties
        {
            get { return GetTrackProperties(Value, _settings); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Value.PropertyChanged -= OnItemPropertyChanged;
                _propertyTrackers.Dispose();
            }
            base.Dispose(disposing);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Changes++;
            var propertyInfo = TrackProperties.SingleOrDefault(x => x.Name == e.PropertyName);
            if (propertyInfo != null)
            {
                _propertyTrackers.RemoveBy(propertyInfo);
                _propertyTrackers.Add((INotifyPropertyChanged)sender, propertyInfo);
            }
        }

        private void OnSubtrackerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ChangesPropertyName)
            {
                Changes++;
            }
        }
    }
}
