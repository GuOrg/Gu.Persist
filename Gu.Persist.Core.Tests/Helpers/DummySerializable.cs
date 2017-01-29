namespace Gu.Persist.Core.Tests
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    [Serializable]
    public class DummySerializable : INotifyPropertyChanged, IEquatable<DummySerializable>
    {
        private int value;

        public DummySerializable()
        {
        }

        public DummySerializable(int value)
        {
            this.Value = value;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }

        public static bool operator ==(DummySerializable left, DummySerializable right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DummySerializable left, DummySerializable right)
        {
            return !Equals(left, right);
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

            return this.value == other.value;
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

            return this.Equals((DummySerializable)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode, hack here but not important as it is just a helper in a test.
            return this.value;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
