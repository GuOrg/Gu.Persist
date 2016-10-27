namespace Gu.Persist.NewtonsoftJson
{
    using Gu.Persist.Core;

    /// <inheritdoc/>
    public sealed class JsonEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        /// <summary>
        /// Returns the default instance.
        /// </summary>
        public new static readonly JsonEqualsComparer<T> Default = new JsonEqualsComparer<T>();

        /// <inheritdoc/>
        protected override byte[] GetBytes(T item)
        {
            using (var stream = JsonFile.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}
