namespace Gu.Persist.RuntimeXml
{
    using System.Collections.Generic;
    using System.IO;
    using Gu.Persist.Core;

    /// <inheritdoc/>
    internal sealed class XmlSerialize : Serialize<XmlRepositorySettings>
    {
        public static readonly XmlSerialize Default = new XmlSerialize();

        /// <inheritdoc/>
        public override Stream ToStream<T>(T item)
        {
            return XmlFile.ToStream(item);
        }

        /// <inheritdoc/>
        public override void ToStream<T>(T item, Stream stream, XmlRepositorySettings settings)
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
                serializer.WriteObject(stream, item);
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