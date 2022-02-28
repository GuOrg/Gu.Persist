namespace Gu.Persist.RuntimeBinary
{
    using Gu.Persist.Core;

    /// <inheritdoc/>
    public sealed class BinaryEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        /// <summary> The default instance. </summary>
        public static new readonly BinaryEqualsComparer<T> Default = new();

        /// <inheritdoc/>
        protected override IPooledStream GetStream(T item)
        {
            return BinaryFile.ToStream(item);
        }
    }
}
