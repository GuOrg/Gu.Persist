namespace Gu.Settings.Core
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Core.Properties;

    internal class FileSettings : IFileSettings
    {
        private DirectoryInfo _directory;
        private string _extension;

        public FileSettings(DirectoryInfo directory, string extension)
        {
            _directory = directory;
            _extension = extension;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public DirectoryInfo Directory
        {
            get { return _directory; }
            private set
            {
                if (Equals(value, _directory))
                {
                    return;
                }
                _directory = value;
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}