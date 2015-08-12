namespace Gu.Settings.Core.Tests.ChangeTracking.Helpers
{
    using System.ComponentModel;

    public class Dummy : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IllegalObject Value { get; set; }

        public class IllegalObject
        {
        }
    }
}
