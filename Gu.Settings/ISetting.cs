namespace Gu.Settings
{
    using System.ComponentModel;

    public interface ISetting : INotifyPropertyChanged
    {
        bool IsDirty { get; set; }
        bool CanSave { get; set; }
    }
}