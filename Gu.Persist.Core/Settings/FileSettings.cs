namespace Gu.Persist.Core
{
    using System;

    /// <summary>
    /// Metadata for reading and saving files.
    /// </summary>
    [Serializable]
    public class FileSettings : IFileSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSettings"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="extension">The extensions.</param>
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