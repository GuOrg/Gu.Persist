namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    [Serializable]
    public class FileSettings : IFileSettings
    {
        public FileSettings(PathAndSpecialFolder directory, string extension)
        {
            this.Directory = directory;
            this.Extension = FileHelper.PrependDotIfMissing(extension);
        }

        public FileSettings(DirectoryInfo directory, string extension)
        {
            this.Directory = PathAndSpecialFolder.Create(directory);
            this.Extension = FileHelper.PrependDotIfMissing(extension);
        }

        protected FileSettings()
        {
        }

        public PathAndSpecialFolder Directory { get; }

        public string Extension { get; }
    }
}