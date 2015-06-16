namespace Gu.Settings
{
    using System;
    using System.ComponentModel;

    public interface ITracker : INotifyPropertyChanged, IDisposable
    {
        event EventHandler Changed;
        
        int Changes { get; }
    }
}