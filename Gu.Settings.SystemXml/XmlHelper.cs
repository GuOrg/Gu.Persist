namespace Gu.Settings.SystemXml
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gu.Settings.Core;

    /// <summary>
    /// Helper methods for serializing and deserializing xml.
    /// </summary>
    public static class XmlHelper
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new ConcurrentDictionary<Type, XmlSerializer>();

        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T FromStream<T>(Stream stream)
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
        public static MemoryStream ToStream<T>(T item)
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

        /// <summary>
        /// Serializes to memorystream, then returns the deserialized object
        /// </summary>
        public static T Clone<T>(T item)
        {
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
            return FileHelper.Read(file, FromStream<T>);
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents
        /// </summary>
        public static Task<T> ReadAsync<T>(FileInfo file)
        {
            return FileHelper.ReadAsync(file, FromStream<T>);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml
        /// </summary>
        public static Task SaveAsync<T>(T item, FileInfo file)
        {
            using (var stream = ToStream(item))
            {
                return FileHelper.SaveAsync(file, stream);
            }
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml
        /// </summary>
        public static void Save<T>(T item, FileInfo file)
        {
            using (var stream = ToStream(item))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}
