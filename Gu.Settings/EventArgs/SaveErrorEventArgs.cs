namespace Gu.Settings
{
    using System;
    using System.IO;

    public class SaveErrorEventArgs : SaveEventArgs
    {
        public SaveErrorEventArgs(object item, FileInfo file, Exception e)
            : base(item, file)
        {
            Exception = e;
        }

        public Exception Exception { get; private set; }
    }
}