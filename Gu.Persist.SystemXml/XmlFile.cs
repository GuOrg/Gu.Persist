namespace Gu.Persist.SystemXml
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    /// <summary>
    /// Helper methods for serializing and deserializing xml.
    /// </summary>
    public static class XmlFile
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new ConcurrentDictionary<Type, XmlSerializer>();

        /// <summary>
        /// Serializes to memorystream, then returns the deserialized object
        /// </summary>
        public static T Clone<T>(T item)
        {
            Ensure.NotNull<object>(item, nameof(item));
            using (var stream = ToStream(item))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return FileHelper.Read(file, FromStream<T>);
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents
        /// </summary>
        public static Task<T> ReadAsync<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return FileHelper.ReadAsync(file, FromStream<T>);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml
        /// </summary>
        public static void Save<T>(FileInfo file, T item)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull<object>(item, nameof(item));
            var serializer = Serializers.GetOrAdd(item.GetType(), x => new XmlSerializer(item.GetType()));
            using (var stream = file.OpenCreate())
            {
                lock (serializer)
                {
                    serializer.Serialize(stream, item);
                }
            }
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml
        /// </summary>
        public static Task SaveAsync<T>(FileInfo file, T item)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull<object>(item, nameof(item));
            using (var stream = ToStream(item))
            {
                return FileHelper.SaveAsync(file, stream);
            }
        }

        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        internal static T FromStream<T>(Stream stream)
        {
            var serializer = Serializers.GetOrAdd(typeof(T), x => new XmlSerializer(typeof(T)));
            lock (serializer)
            {
                var setting = (T)serializer.Deserialize(stream);
                return setting;
            }
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>
        /// </summary>
        internal static MemoryStream ToStream<T>(T item)
        {
            var ms = new MemoryStream();
            var serializer = Serializers.GetOrAdd(item.GetType(), x => new XmlSerializer(item.GetType()));
            lock (serializer)
            {
                serializer.Serialize(ms, item);
            }

            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        internal static XmlSerializer SerializerFor(object item)
        {
            return Serializers.GetOrAdd(item.GetType(), x => new XmlSerializer(item.GetType()));
        }
    }
}
