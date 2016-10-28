namespace Gu.Persist.RuntimeXml
{
    using System.IO;
    using Gu.Persist.Core;

    public sealed class XmlEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        public new static readonly XmlEqualsComparer<T> Default = new XmlEqualsComparer<T>();

        /// <inheritdoc/>
        protected override MemoryStream GetStream(T item)
        {
            return XmlFile.ToStream(item);
        }
    }
}
