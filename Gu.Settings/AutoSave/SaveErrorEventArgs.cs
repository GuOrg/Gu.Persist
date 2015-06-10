namespace Gu.Settings
{
    using System;

    public class SaveErrorEventArgs : SaveEventArgs
    {
        public SaveErrorEventArgs(object item, IFileInfos fileInfos, Exception e)
            : base(item, fileInfos)
        {
            Exception = e;
        }

        public Exception Exception { get; private set; }
    }
}