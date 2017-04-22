namespace Gu.Persist.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class AutoSaveSetting : INotifyPropertyChanged
    {
        public static readonly IReadOnlyList<StringComparison> AllComparisons =
            Enum.GetValues(typeof(StringComparison)).Cast<StringComparison>().ToArray();

        private int value1;
        private int value2;
        private StringComparison comparison;

        private AutoSaveSetting()
        {
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public int Value1
        {
            get => this.value1;
            set
            {
                if (value == this.value1)
                {
                    return;
                }

                this.value1 = value;
                this.OnPropertyChanged();
            }
        }

        public int Value2
        {
            get => this.value2;
            set
            {
                if (value == this.value2)
                {
                    return;
                }

                this.value2 = value;
                this.OnPropertyChanged();
            }
        }

        public StringComparison Comparison
        {
            get => this.comparison;
            set
            {
                if (value == this.comparison)
                {
                    return;
                }

                this.comparison = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
