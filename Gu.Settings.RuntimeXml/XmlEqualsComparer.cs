namespace Gu.Settings.RuntimeXml
{
    using Gu.Settings.Core;

    public sealed class XmlEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        public new static readonly XmlEqualsComparer<T> Default = new XmlEqualsComparer<T>();

        /// <inheritdoc/>
        protected override byte[] GetBytes(T item)
        {
            using (var stream = XmlHelper.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}
