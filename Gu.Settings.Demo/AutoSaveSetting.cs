namespace Gu.Settings.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Demo.Annotations;

    [Serializable]
    public class AutoSaveSetting : INotifyPropertyChanged
    {
        public static AutoSaveSetting Instance = new AutoSaveSetting();
        private int _value1 = 1;
        private int _value2 = 2;

        private AutoSaveSetting()
        {
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public int Value1
        {
            get { return _value1; }
            set
            {
                _value1 = value;
                OnPropertyChanged();
            }
        }

        public int Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
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
