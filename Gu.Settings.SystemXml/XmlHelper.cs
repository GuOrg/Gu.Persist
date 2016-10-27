namespace Gu.Settings.SystemXml
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gu.Settings.Core;

    public static class XmlHelper
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new ConcurrentDictionary<Type, XmlSerializer>();

        public static T FromStream<T>(Stream stream)
        {
            var serializer = Serializers.GetOrAdd(typeof(T), x => new XmlSerializer(typeof(T)));
            lock (serializer)
            {
                var setting = (T)serializer.Deserialize(stream);
                return setting;
            }
        }

        public static MemoryStream ToStream<T>(T o)
        {
            var ms = new MemoryStream();
            var serializer = Serializers.GetOrAdd(o.GetType(), x => new XmlSerializer(o.GetType()));
            lock (serializer)
            {
                serializer.Serialize(ms, o);
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
        /// Reads an xml file and deserialize the contents
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

        public static Task SaveAsync<T>(T o, FileInfo file)
        {
            using (var stream = ToStream(o))
            {
                return FileHelper.SaveAsync(file, stream);
            }
        }

        public static void Save<T>(T o, FileInfo file)
        {
            using (var stream = ToStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}
