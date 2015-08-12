namespace Gu.Settings.Core
{
    using System;
    using System.ComponentModel;

    public interface ITracker : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// This event is raised when a change in the tracked intance is detected.
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// Gets a value that is incremented each time a change is detected in the tracked instance.
        /// </summary>
        int Changes { get; }
    }
}