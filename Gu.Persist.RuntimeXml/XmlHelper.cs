namespace Gu.Persist.RuntimeXml
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Gu.Persist.Core;

    public static class XmlHelper
    {
        private static readonly ConcurrentDictionary<Type, DataContractSerializer> Serializers = new ConcurrentDictionary<Type, DataContractSerializer>();

        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T FromStream<T>(Stream stream)
        {
            var serializer = Serializers.GetOrAdd(typeof(T), x => new DataContractSerializer(typeof(T)));
            lock (serializer)
            {
                var setting = (T)serializer.ReadObject(stream);
                return setting;
            }
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>
        /// </summary>
        public static MemoryStream ToStream<T>(T item)
        {
            var ms = new MemoryStream();
            var serializer = Serializers.GetOrAdd(item.GetType(), x => new DataContractSerializer(item.GetType()));
            lock (serializer)
            {
                serializer.WriteObject(ms, item);
            }

            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Read the contents of <paramref name="file"/> and serialize it to <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(FileInfo file)
        {
            return FileHelper.Read(file, FromStream<T>);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static Task<T> ReadAsync<T>(FileInfo file)
        {
            return FileHelper.ReadAsync(file, FromStream<T>);
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
    }
}