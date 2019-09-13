namespace Gu.Persist.RuntimeXml
{
    using Gu.Persist.Core;

    /// <inheritdoc/>
    public sealed class XmlEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        public new static readonly XmlEqualsComparer<T> Default = new XmlEqualsComparer<T>();

        /// <inheritdoc/>
        protected override IPooledStream GetStream(T item)
        {
            return XmlFile.ToStream(item);
        }
    }
}
