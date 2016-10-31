namespace Gu.Persist.RuntimeXml
{
    using Gu.Persist.Core;

    /// <inheritdoc/>
    public sealed class XmlEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        public new static readonly XmlEqualsComparer<T> Default = new XmlEqualsComparer<T>();

        /// <inheritdoc/>
        protected override IPooledStream GetStream(T item)
        {
            return File.ToStream(item);
        }
    }
}
