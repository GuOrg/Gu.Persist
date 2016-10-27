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

        public object Item { get; private set; }

        public FileInfo File { get; private set; }
    }
}