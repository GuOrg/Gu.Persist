namespace Gu.Persist.Core
{
    using System;

    /// <summary>
    /// Metadata for reading and saving files.
    /// </summary>
    [Serializable]
    public class FileSettings : IFileSettings
    {
        public FileSettings(string directory, string extension)
        {
            Ensure.NotNull(extension, nameof(extension));
            this.Directory = directory ?? throw new ArgumentNullException(nameof(directory));
            this.Extension = FileHelper.PrependDotIfMissing(extension);
        }

        /// <summary>
        /// Gets the path to the directory.
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public string Extension { get; }
    }
}