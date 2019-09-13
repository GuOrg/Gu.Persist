namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    /// <inheritdoc/>
    public class SaveEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item that was saved.</param>
        /// <param name="file">The file.</param>
        public SaveEventArgs(object item, FileInfo file)
        {
            this.Item = item;
            this.File = file;
        }

        /// <summary>
        /// Gets the item that was saved.
        /// </summary>
        public object Item { get; }

        /// <summary>
        /// Gets the file.
        /// </summary>
        public FileInfo File { get; }
    }
}