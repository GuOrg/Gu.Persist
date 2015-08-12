namespace Gu.Settings
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    [DebuggerDisplay("Type: {TypeName} TrackAs: {TrackAs}")]
    public class ChangeTrackerSpecialType : INotifyPropertyChanged
    {
        private string _typeName;
        private TrackAs _trackAs;

        protected ChangeTrackerSpecialType() // for serialization
        {
        }

        public ChangeTrackerSpecialType(string fullTypeName, TrackAs trackAs)
        {
            Ensure.NotNullOrEmpty(fullTypeName, nameof(fullTypeName));
            Ensure.NotEqual(trackAs, TrackAs.Unknown, "trackAs");
            _typeName = fullTypeName;
            _trackAs = trackAs;
        }

        public ChangeTrackerSpecialType(Type type, TrackAs trackAs)
        {
            Ensure.NotNull(type, nameof(type));
            Ensure.NotEqual(trackAs, TrackAs.Unknown, "trackAs");
            _typeName = type.FullName;
            _trackAs = trackAs;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Gets or sets the full name of the type
        /// </summary>
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                if (value == _typeName)
                {
                    return;
                }
                _typeName = value;
                OnPropertyChanged();
            }
        }

        public TrackAs TrackAs
        {
            get { return _trackAs; }
            set
            {
                if (value == _trackAs)
                {
                    return;
                }
                _trackAs = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}