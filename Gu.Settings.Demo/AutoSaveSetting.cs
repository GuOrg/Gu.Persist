namespace Gu.Settings.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Demo.Annotations;

    [Serializable]
    public class AutoSaveSetting : INotifyPropertyChanged
    {
        public static readonly IReadOnlyList<StringComparison> AllComparisons =
            Enum.GetValues(typeof (StringComparison)).Cast<StringComparison>().ToArray(); 
        public static AutoSaveSetting Instance = new AutoSaveSetting();
        private int _value1 = 1;
        private int _value2 = 2;
        private StringComparison _comparison;

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

        public StringComparison Comparison
        {
            get { return _comparison; }
            set
            {
                _comparison = value;
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
