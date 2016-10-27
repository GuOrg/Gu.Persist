namespace Gu.Persist.SystemXml
{
    using Gu.Persist.Core;

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
            using (var stream = XmlFile.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}