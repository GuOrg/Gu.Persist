namespace Gu.Settings
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    internal abstract class ValidatingSetting<T> : ISetting
    {
        private bool _isValid;
        private Setting<T> _setting;
        private IDisposable _isValidSubscription;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (value.Equals(_isValid))
                {
                    return;
                }
                _isValid = value;
                OnPropertyChanged();
            }
        }

        public Setting<T> Setting
        {
            get { return _setting; }
            set
            {
                throw new NotImplementedException("message");
                
                //if (_isValidSubscription != null)
                //{
                //    _isValidSubscription.Dispose();
                //}
                //_setting = value;
                //if(_setting != null)
                //{
                //    _isValidSubscription = _setting.ObservePropertyChanged()
                //                               .Subscribe(_ => IsValid = IsValidCore(_setting.EditValue));
                //}
            }
        }

        public bool IsDirty { get; private set; }

        protected abstract bool IsValidCore(T value);

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