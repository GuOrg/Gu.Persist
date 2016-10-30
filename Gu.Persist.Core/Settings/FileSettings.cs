namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    [Serializable]
    public class FileSettings : IFileSettings
    {
        public FileSettings(PathAndSpecialFolder directoryPath, string extension)
        {
            this.DirectoryPath = directoryPath;
            this.Extension = FileHelper.PrependDotIfMissing(extension);
        }

        public FileSettings(DirectoryInfo directory, string extension)
        {
            this.DirectoryPath = PathAndSpecialFolder.Create(directory);
            this.Extension = FileHelper.PrependDotIfMissing(extension);
        }

        protected FileSettings()
        {
        }

        public PathAndSpecialFolder DirectoryPath { get; }

        public string Extension { get; }
    }
}