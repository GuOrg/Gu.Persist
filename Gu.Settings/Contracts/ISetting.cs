﻿namespace Gu.Settings
{
    using System.ComponentModel;

    public interface ISetting : INotifyPropertyChanged
    {
        bool IsDirty { get; }
    }
}