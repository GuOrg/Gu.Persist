namespace Gu.Settings.Core.Tests.ChangeTracking.Helpers
{
    using System.Collections;
    using System.ComponentModel;

    public class IllegalEnumerable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable Value { get; set; }
    }
}