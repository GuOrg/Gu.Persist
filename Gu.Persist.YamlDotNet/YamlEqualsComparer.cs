namespace Gu.Persist.Yaml
{
    using Gu.Persist.Core;

    /// <inheritdoc/>
    public sealed class YamlEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        public new static readonly YamlEqualsComparer<T> Default = new YamlEqualsComparer<T>();

        /// <inheritdoc/>
        protected override IPooledStream GetStream(T item)
        {
            return YamlFile.ToStream(item);
        }
    }
}
