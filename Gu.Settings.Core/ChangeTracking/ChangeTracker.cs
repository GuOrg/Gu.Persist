namespace Gu.Settings.Core
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public abstract class ChangeTracker : ITracker
    {
        protected static readonly PropertyInfo ChangesPropertyInfo = typeof(ChangeTracker).GetProperty(nameof(Changes));
        protected static readonly PropertyChangedEventArgs ChangesEventArgs = new PropertyChangedEventArgs(nameof(Changes));
        private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>> TrackPropertiesMap = new ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>>();
        private int _changes;
        private bool _disposed;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public event EventHandler Changed;

        /// <inheritdoc/>
        public int Changes
        {
            get { return _changes; }
            protected set
            {
                if (value == _changes)
                {
                    return;
                }
                _changes = value;
                OnPropertyChanged(ChangesEventArgs);
                OnChanged();
            }
        }

        /// <summary>
        /// Creates a tracker that detects and notifies about changes of any property or subproperty of <paramref name="root"/>
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static IValueTracker Track(INotifyPropertyChanged root)
        {
            Ensure.NotNull(root, nameof(root));
            return Track(root, ChangeTrackerSettings.Default);
        }

        /// <summary>
        /// Creates a tracker that detects and notifies about changes of any property or subproperty of <paramref name="root"/>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IValueTracker Track(INotifyPropertyChanged root, ChangeTrackerSettings settings)
        {
            Ensure.NotNull(root, nameof(root));
            Ensure.NotNull(settings, nameof(settings));

            var tracker = Create(typeof(ChangeTracker), ChangesPropertyInfo, root, settings);
            Ensure.NotNull(tracker, nameof(tracker));
            return tracker;
        }

        internal static void Verify(Type parentType, PropertyInfo parentProperty, object item, ChangeTrackerSettings settings)
        {
            if (Attribute.IsDefined(parentType, typeof(TrackingAttribute), true))
            {
                return;
            }

            if (Attribute.IsDefined(parentProperty, typeof(TrackingAttribute), true))
            {
                return;
            }

            var propertyType = parentProperty.PropertyType;
            if (settings.SpecialTypes.Any(x => x.TypeName == propertyType.FullName))
            {
                return;
            }
            //if (Attribute.IsDefined(propertyType, typeof(TrackingAttribute), true))
            //{
            //    return;
            //}

            if (!IsTrackType(propertyType, settings))
            {
                return;
            }

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(propertyType))
            {
                return;
            }

            if (typeof(INotifyCollectionChanged).IsAssignableFrom(propertyType))
            {
                return;
            }
            // settings.AddSpecialType<FileInfo>(TrackAs.Explicit)
            var message =
                string.Format(
                    @"Create tracker failed for {0}.{1}." + Environment.NewLine +
                    @"Solve the problem by:" +Environment.NewLine +
                    @"1) Add a specialcase to tracker setting example: " + Environment.NewLine +
                    @"    settings.AddSpecialType<YourType>(TrackAs.Explicit)" + Environment.NewLine +
                    @"    Note that this requires you to track changes." + Environment.NewLine +
                    @"2) Implementing INotifyPropertyChanged for {2}" + Environment.NewLine +
                    @"3) Implementing INotifyCollectionChanged for {2}" +Environment.NewLine +
                    @"4) Add TrackingAttribute: Immutable to type:{1}" + Environment.NewLine +
                    @"5) Add TrackingAttribute: Explicit to {2} ",
                    parentType.Name,
                    parentProperty.Name,
                    parentProperty.PropertyType.Name);
            throw new ArgumentException(message);
        }

        internal static bool CanTrack(Type parentType, PropertyInfo parentProperty, object value, ChangeTrackerSettings settings)
        {
            Verify(parentType, parentProperty, value, settings);

            var incc = value as INotifyCollectionChanged;
            if (incc != null)
            {
                return true;
            }
            var inpc = value as INotifyPropertyChanged;
            if (inpc != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected static IPropertyTracker Create(Type parentType, PropertyInfo parentProperty, object child, ChangeTrackerSettings settings)
        {
            if (!CanTrack(parentType, parentProperty, child, settings))
            {
                return null;
            }

            if (child == null)
            {
                return null;
            }
            var incc = child as INotifyCollectionChanged;
            if (incc != null)
            {
                return new CollectionTracker(parentType, parentProperty, (IEnumerable)incc, settings);
            }
            var inpc = child as INotifyPropertyChanged;
            if (inpc != null)
            {
                return new PropertyChangeTracker(parentType, parentProperty, inpc, settings);
            }
            throw new ArgumentException();
        }

        protected static IReadOnlyList<PropertyInfo> GetTrackProperties(INotifyPropertyChanged item, ChangeTrackerSettings settings)
        {
            if (item == null)
            {
                return null;
            }

            var trackProperties = TrackPropertiesMap.GetOrAdd(item.GetType(), t => TrackPropertiesFor(t, settings));
            return trackProperties;
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (disposing)
            {
                // Free any other managed objects here. 
            }

            // Free any unmanaged objects here. 
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnChanged()
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected static bool IsTrackType(Type type, ChangeTrackerSettings settings)
        {
            Ensure.NotNull(type, nameof(type));
            if (type == typeof(string) || type.IsEnum || type.IsPrimitive)
            {
                return false;
            }

            if (settings.SpecialTypes.Any(x => x.TypeName == type.FullName))
            {
                return false;
            }
            return true;
        }

        private static IReadOnlyList<PropertyInfo> TrackPropertiesFor(Type type, ChangeTrackerSettings settings)
        {
            var propertyInfos = type.GetProperties()
                                    .Where(x => x.GetIndexParameters().Length == 0 && IsTrackType(x.PropertyType, settings))
                                    .ToArray();
            return propertyInfos;
        }
    }
}