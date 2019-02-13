namespace Gu.Persist.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class ManualSaveSetting : INotifyPropertyChanged
    {
        private int value1 = 1;
        private int value2 = 2;
        private StringComparison comparison;

        private ManualSaveSetting()
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