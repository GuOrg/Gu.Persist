namespace Gu.Persist.SystemXml
{
    using System.Collections.Generic;
    using System.IO;

    /// <inheritdoc/>
    internal sealed class Serialize<TSetting> : Gu.Persist.Core.Serialize<TSetting>
        where TSetting : Core.RepositorySettings
    {
        public static readonly Serialize<TSetting> Default = new Serialize<TSetting>();

        /// <inheritdoc/>
        public override Stream ToStream<T>(T item)
        {
            return XmlFile.ToStream(item);
        }

        /// <inheritdoc/>
        public override void ToStream<T>(T item, Stream stream, TSetting settings)
        {
            var source = item as Stream;
            if (source != null)
            {
                source.CopyTo(stream);
                return;
            }

            var serializer = XmlFile.SerializerFor(item);
            lock (serializer)
            {
                serializer.Serialize(stream, item);
            }
        }

        /// <inheritdoc/>
        public override T FromStream<T>(Stream stream)
        {
            return XmlFile.FromStream<T>(stream);
        }

        /// <inheritdoc/>
        public override T Clone<T>(T item)
        {
            return XmlFile.Clone(item);
        }

        /// <inheritdoc/>
        public override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return XmlEqualsComparer<T>.Default;
        }
    }
}