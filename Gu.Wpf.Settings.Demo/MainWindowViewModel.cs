namespace Gu.Wpf.Settings.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Settings.Annotations;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private SampleSetting _setting;

        public MainWindowViewModel()
        {
            _setting = new SampleSetting();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SampleSetting Setting
        {
            get { return _setting; }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
