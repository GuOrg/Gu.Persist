namespace Gu.Persist.Core
{
    using System;

    /// <summary>
    /// Metadata for reading and saving files.
    /// </summary>
    [Serializable]
    public class FileSettings : IFileSettings
    {
        public FileSettings(PathAndSpecialFolder directory, string extension)
        {
            Ensure.NotNull(directory, nameof(directory));
            Ensure.NotNull(extension, nameof(extension));
            this.Directory = directory;
            this.Extension = FileHelper.PrependDotIfMissing(extension);
        }

        protected FileSettings()
        {
        }

        public PathAndSpecialFolder Directory { get; }

        public string Extension { get; }
    }
}