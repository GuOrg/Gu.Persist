namespace Gu.Persist.RuntimeXml
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Gu.Persist.Core;

    /// <summary>
    /// Helper class for serializing and deserializing using <see cref="DataContractSerializer"/>
    /// </summary>
    public static class XmlFile
    {
        private static readonly ConcurrentDictionary<Type, DataContractSerializer> Serializers = new ConcurrentDictionary<Type, DataContractSerializer>();

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
        /// Read the contents of <paramref name="file"/> and serialize it to <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return FileHelper.Read(file, FromStream<T>);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
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
            var serializer = Serializers.GetOrAdd(item.GetType(), x => new DataContractSerializer(item.GetType()));

            using (var stream = file.OpenCreate())
            {
                lock (serializer)
                {
                    serializer.WriteObject(stream, item);
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
            var serializer = Serializers.GetOrAdd(typeof(T), x => new DataContractSerializer(typeof(T)));
            lock (serializer)
            {
                var setting = (T)serializer.ReadObject(stream);
                return setting;
            }
        }

        internal static DataContractSerializer SerializerFor<T>(T item)
        {
            return Serializers.GetOrAdd(item.GetType(), x => new DataContractSerializer(item.GetType()));
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>
        /// </summary>
        internal static MemoryStream ToStream<T>(T item)
        {
            var ms = PooledMemoryStream.Borrow();
            var serializer = Serializers.GetOrAdd(item.GetType(), x => new DataContractSerializer(item.GetType()));
            lock (serializer)
            {
                serializer.WriteObject(ms, item);
            }

            ms.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}