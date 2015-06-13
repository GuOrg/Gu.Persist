namespace Gu.Settings
{
    using System.ComponentModel;
    using System.IO;

    public interface IFileSettings : INotifyPropertyChanged
    {
        DirectoryInfo Directory { get; }

        string Extension { get; }
    }
}