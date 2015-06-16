namespace Gu.Settings.Tests.ChangeTracking.Helpers
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    public class Level : INotifyPropertyChanged
    {
        private int _value;
        private Level _next;
        private ObservableCollection<int> _ints = new ObservableCollection<int>();
        private ObservableCollection<Level> _levels = new ObservableCollection<Level>();
        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get { return _value; }
            set
            {
                if (value == _value)
                {
                    return;
                }
                _value = value;
                OnPropertyChanged();
            }
        }

        public Level Next
        {
            get { return _next; }
            set
            {
                if (Equals(value, _next))
                {
                    return;
                }
                _next = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Ints
        {
            get { return _ints; }
            set
            {
                if (Equals(value, _ints))
                {
                    return;
                }
                _ints = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Level> Levels
        {
            get { return _levels; }
            set
            {
                if (Equals(value, _levels))
                {
                    return;
                }
                _levels = value;
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
