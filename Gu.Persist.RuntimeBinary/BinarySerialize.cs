namespace Gu.Persist.RuntimeBinary
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Gu.Persist.Core;

    /// <inheritdoc/>
    internal sealed class BinarySerialize : Serialize<BinaryRepositorySettings>
    {
        public static readonly BinarySerialize Default = new BinarySerialize();

        /// <inheritdoc/>
        public override Stream ToStream<T>(T item)
        {
            return BinaryFile.ToStream(item);
        }

        /// <inheritdoc/>
        public override void ToStream<T>(T item, Stream stream, BinaryRepositorySettings settings)
        {
            var source = item as Stream;
            if (source != null)
            {
                source.CopyTo(stream);
                return;
            }

            var serializer = new BinaryFormatter();
            serializer.Serialize(stream, item);
        }

        /// <inheritdoc/>
        public override T FromStream<T>(Stream stream)
        {
            return BinaryFile.FromStream<T>(stream);
        }

        /// <inheritdoc/>
        public override T Clone<T>(T item)
        {
            return BinaryFile.Clone(item);
        }

        /// <inheritdoc/>
        public override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return BinaryEqualsComparer<T>.Default;
        }
    }
}