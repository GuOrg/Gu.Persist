namespace Gu.Settings.SystemXml
{
    using Gu.Settings.Core;

    /// <inheritdoc/>
    public class XmlEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
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