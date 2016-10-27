namespace Gu.Settings.NewtonsoftJson
{
    using Gu.Settings.Core;

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
            using (var stream = JsonHelper.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}
