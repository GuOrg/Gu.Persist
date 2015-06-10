namespace Gu.Settings.Tests.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    [Serializable]
    public class DummySerializable : INotifyPropertyChanged, IEquatable<DummySerializable>
    {
        private int _value;

        public DummySerializable()
        {
        }

        public DummySerializable(int value)
        {
            Value = value;
        }

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

        public bool Equals(DummySerializable other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((DummySerializable)obj);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(DummySerializable left, DummySerializable right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DummySerializable left, DummySerializable right)
        {
            return !Equals(left, right);
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
