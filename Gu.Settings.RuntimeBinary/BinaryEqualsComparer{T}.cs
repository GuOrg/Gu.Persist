namespace Gu.Settings.RuntimeBinary
{
    using Gu.Settings.Core;

    /// <inheritdoc/>
    public class BinaryEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        /// <summary> The default instance. </summary>
        public new static readonly BinaryEqualsComparer<T> Default = new BinaryEqualsComparer<T>();

        /// <inheritdoc/>
        protected override byte[] GetBytes(T item)
        {
            using (var stream = BinaryHelper.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}
