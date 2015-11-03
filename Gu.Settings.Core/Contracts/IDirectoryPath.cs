namespace Gu.Settings.Core
{
    using System;
    using System.ComponentModel;
    using System.IO;

    public interface IDirectoryPath : INotifyPropertyChanged
    {
        DirectoryInfo Directory { get; }

        /// <summary>
        /// Absolute or relative path.
        /// ./Settings is relative to current directory
        /// Relative to SpecialFolder
        /// </summary>
        string Path { get; }

        Environment.SpecialFolder? SpecialFolder { get; }
    }
}