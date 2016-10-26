namespace Gu.Settings.Core
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    [Serializable]
    public class FileSettings : IFileSettings
    {
        private string extension;
        private PathAndSpecialFolder directoryPath;

        protected FileSettings()
        {
        }

        public FileSettings(PathAndSpecialFolder directoryPath, string extension)
        {
            this.directoryPath = directoryPath;
            this.extension = FileHelper.PrependDotIfMissing(extension);
        }

        public FileSettings(DirectoryInfo directory, string extension)
        {
            this.directoryPath = PathAndSpecialFolder.Create(directory);
            this.extension = FileHelper.PrependDotIfMissing(extension);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public PathAndSpecialFolder DirectoryPath
        {
            get
            {
                return this.directoryPath;
            }

            set
            {
                if (Equals(value, this.directoryPath))
                {
                    return;
                }

                this.directoryPath = value;
                this.OnPropertyChanged();
            }
        }

        public string Extension
        {
            get
            {
                return this.extension;
            }

            protected set
            {
                if (value == this.extension)
                {
                    return;
                }

                this.extension = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}