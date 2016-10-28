namespace Gu.Persist.RuntimeBinary
{
    using System.IO;
    using Gu.Persist.Core;

    /// <inheritdoc/>
    public class BinaryEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        /// <summary> The default instance. </summary>
        public new static readonly BinaryEqualsComparer<T> Default = new BinaryEqualsComparer<T>();

        /// <inheritdoc/>
        protected override MemoryStream GetStream(T item)
        {
            return BinaryFile.ToStream(item);
        }
    }
}
