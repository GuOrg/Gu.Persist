namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    /// <inheritdoc/>
    public class SaveEventArgs : EventArgs
    {
        public SaveEventArgs(object item, FileInfo file)
        {
            this.Item = item;
            this.File = file;
        }

        public object Item { get; }

        public FileInfo File { get; }
    }
}