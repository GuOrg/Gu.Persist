namespace Gu.Settings
{
    using System.ComponentModel;

    /// <summary>
    /// Internal until done
    /// </summary>
    internal interface ISetting : INotifyPropertyChanged
    {
        bool IsDirty { get; }
    }
}