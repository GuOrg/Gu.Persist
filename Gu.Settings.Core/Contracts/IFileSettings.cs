﻿namespace Gu.Settings.Core
{
    using System.ComponentModel;

    public interface IFileSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the path to where backups are saved.
        /// </summary>
        PathAndSpecialFolder DirectoryPath { get; }

        /// <summary>
        /// Gets the extension
        /// </summary>
        string Extension { get; }
    }
}