namespace Gu.Persist.SystemXml
{
    using System.Collections.Generic;
    using System.IO;

    /// <inheritdoc/>
    internal sealed class Serialize<TSetting> : Gu.Persist.Core.Serialize<TSetting>
        where TSetting : Core.RepositorySettings
    {
        internal static readonly Serialize<TSetting> Default = new();

        /// <inheritdoc/>
        public override Stream ToStream<T>(T item, TSetting setting)
        {
            return XmlFile.ToStream(item);
        }

        /// <inheritdoc/>
        public override void ToStream<T>(T item, Stream stream, TSetting settings)
        {
            switch (item)
            {
                case null:
                    return;
                case Stream source:
                    source.CopyTo(stream);
                    return;
            }

            var serializer = XmlFile.SerializerFor(item!);
            lock (serializer)
            {
                serializer.Serialize(stream, item);
            }
        }

        /// <inheritdoc/>
        public override T FromStream<T>(Stream stream, TSetting setting)
        {
            return XmlFile.FromStream<T>(stream);
        }

        /// <inheritdoc/>
        public override T Clone<T>(T item, TSetting setting)
        {
            return XmlFile.Clone(item);
        }

        /// <inheritdoc/>
        public override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>(TSetting setting)
        {
            return XmlEqualsComparer<T>.Default;
        }
    }
}