namespace Gu.Persist.RuntimeXml
{
    using Gu.Persist.Core;

    public sealed class XmlEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        public new static readonly XmlEqualsComparer<T> Default = new XmlEqualsComparer<T>();

        /// <inheritdoc/>
        protected override byte[] GetBytes(T item)
        {
            using (var stream = XmlFile.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}
