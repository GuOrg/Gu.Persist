namespace Gu.Settings.RuntimeBinary
{
    using Gu.Settings.Core;

    public class BinaryEqualsComparer<T> : SerializedEqualsComparer<T>
    {
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
