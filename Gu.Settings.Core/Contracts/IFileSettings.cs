namespace Gu.Settings.Core
{
    using System.ComponentModel;
    using System.IO;

    public interface IFileSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the directory.
        /// </summary>
        DirectoryInfo Directory { get; }

        /// <summary>
        /// Gets the extension
        /// </summary>
        string Extension { get; }
    }
}