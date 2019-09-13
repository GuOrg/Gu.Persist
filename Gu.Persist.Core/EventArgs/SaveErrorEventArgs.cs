namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    /// <inheritdoc/>
    public class SaveErrorEventArgs : SaveEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveErrorEventArgs"/> class.
        /// </summary>
        /// <param name="item">The saved item.</param>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="e">The <see cref="Exception"/>.</param>
        public SaveErrorEventArgs(object item, FileInfo file, Exception e)
            : base(item, file)
        {
            this.Exception = e;
        }

        /// <summary>
        /// Gets the <see cref="Exception"/>.
        /// </summary>
        public Exception Exception { get; }
    }
}