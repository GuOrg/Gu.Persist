namespace Gu.Settings
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    [Serializable]
    internal class Setting<T> : ISetting
    {
        private T _editValue;
        private T _value;
        private T _oldValue;
        private bool _isDirty;
        private bool _canSave;

        public event PropertyChangedEventHandler PropertyChanged;

        public Setting(bool writeDirectlyToTarget)
        {
            WriteDirectlyToTarget = writeDirectlyToTarget;
        }

        public bool WriteDirectlyToTarget { get; set; }

        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                if (value.Equals(_canSave))
                {
                    return;
                }
                _canSave = value;
                OnPropertyChanged();
            }
        }

        public T EditValue
        {
            get { return _editValue; }
            set
            {
                if (Equals(value, _editValue))
                {
                    return;
                }
                OldValue = _editValue;
                _editValue = value;
                if (WriteDirectlyToTarget)
                {
                    Value = _editValue;
                }
                IsDirty = !Equals(_editValue, Value);
                OnPropertyChanged();
            }
        }

        public T Value
        {
            get { return _value; }
            set
            {
                if (Equals(value, _value))
                {
                    return;
                }
                _value = value;
                OnPropertyChanged();
            }
        }

        public T OldValue
        {
            get { return _oldValue; }
            set
            {
                if (Equals(value, _oldValue))
                {
                    return;
                }
                _oldValue = value;
                OnPropertyChanged();
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value.Equals(_isDirty))
                {
                    return;
                }
                _isDirty = value;
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
