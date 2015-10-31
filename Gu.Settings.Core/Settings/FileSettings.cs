namespace Gu.Settings.Core
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class FileSettings : IFileSettings
    {
        private string _extension;
        private DirectoryPath _directoryPath;

        protected FileSettings()
        {
        }

        protected FileSettings(DirectoryPath directoryPath, string extension)
        {
            _directoryPath = directoryPath;
            _extension = FileHelper.PrependDotIfMissing(extension);
        }

        public FileSettings(DirectoryInfo directory, string extension)
        {
            _directoryPath = new DirectoryPath(directory);
            _extension = FileHelper.PrependDotIfMissing(extension);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public DirectoryPath DirectoryPath
        {
            get { return _directoryPath; }
            set
            {
                if (Equals(value, _directoryPath))
                {
                    return;
                }
                _directoryPath = value;
                OnPropertyChanged();
            }
        }

        public string Extension
        {
            get { return _extension; }
            private set
            {
                if (value == _extension)
                {
                    return;
                }
                _extension = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}