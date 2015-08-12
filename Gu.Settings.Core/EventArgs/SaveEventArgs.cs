namespace Gu.Settings.Core
{
    using System;
    using System.IO;

    public class SaveEventArgs : EventArgs
    {
        public SaveEventArgs(object item, FileInfo file)
        {
            Item = item;
            File = file;
        }
        public object Item { get; private set; }

        public FileInfo File { get; private set; }
    }
}