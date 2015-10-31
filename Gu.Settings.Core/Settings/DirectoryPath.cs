namespace Gu.Settings.Core
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class DirectoryPath : IDirectoryPath
    {
        public static readonly DirectoryPath Default = new DirectoryPath(Directories.DefaultPath);
        [NonSerialized]
        private DirectoryInfo _directory;
        private PathAndSpecialFolder _path;

        public DirectoryPath(PathAndSpecialFolder path)
        {
            _directory = path.CreateDirectoryInfo();
            _path = path;
        }

        public DirectoryPath(DirectoryInfo directory)
        {
            _directory = directory;
            _path = PathAndSpecialFolder.Create(directory);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DirectoryInfo Directory
        {
            get { return _directory ?? (_directory = _path.CreateDirectoryInfo()); }
            private set
            {
                if (DirectoryInfoComparer.Default.Equals(value, _directory))
                {
                    return;
                }
                _directory = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get { return _path?.Path; }
            set
            {
                if (value == _path?.Path)
                {
                    return;
                }
                _path = new PathAndSpecialFolder(value, _path?.SpecialFolder);

                UpdateDirectory();
                OnPropertyChanged();
            }
        }

        public Environment.SpecialFolder? SpecialFolder
        {
            get { return _path?.SpecialFolder; }
            set
            {
                if (value == _path?.SpecialFolder)
                {
                    return;
                }
                _path = new PathAndSpecialFolder(_path?.Path, value);
                UpdateDirectory();
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateDirectory()
        {
            if (_path?.CanCreateDirectoryInfo != true)
            {
                Directory = null;
                return;
            }
            var dir = _path.CreateDirectoryInfo();
            if (!DirectoryInfoComparer.Default.Equals(dir, _directory))
            {
                _directory = dir;
                OnPropertyChanged(nameof(Directory));
            }
        }
    }
}