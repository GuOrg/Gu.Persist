namespace Gu.Settings
{
    using System;

    public class SaveEventArgs : EventArgs
    {
        public SaveEventArgs(object item, IFileInfos fileInfos)
        {
            Item = item;
            FileInfos = fileInfos;
        }
        public object Item { get; private set; }
        public IFileInfos FileInfos { get; private set; }
    }
}