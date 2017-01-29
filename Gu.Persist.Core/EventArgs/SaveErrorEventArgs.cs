namespace Gu.Persist.Core
{
    using System;
    using System.IO;

    /// <inheritdoc/>
    public class SaveErrorEventArgs : SaveEventArgs
    {
        public SaveErrorEventArgs(object item, FileInfo file, Exception e)
            : base(item, file)
        {
            this.Exception = e;
        }

        public Exception Exception { get; }
    }
}