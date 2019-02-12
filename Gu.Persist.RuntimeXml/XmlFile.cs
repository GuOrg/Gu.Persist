namespace Gu.Persist.RuntimeXml
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Gu.Persist.Core;

    /// <summary>
    /// Helper class for serializing and deserializing using <see cref="DataContractSerializer"/>.
    /// </summary>
    public static class XmlFile
    {
        private static readonly ConcurrentDictionary<Type, DataContractSerializer> Serializers = new ConcurrentDictionary<Type, DataContractSerializer>();

        /// <summary>
        /// Serializes to memorystream, then returns the deserialized object.
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
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        public static T Read<T>(string fileName)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            using (var stream = File.OpenRead(fileName))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// Read the contents of <paramref name="file"/> and serialize it to <typeparamref name="T"/>.
        /// </summary>
        public static T Read<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return Read<T>(file.FullName);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        public static async Task<T> ReadAsync<T>(string fileName)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            using (var stream = await FileHelper.ReadAsync(fileName).ConfigureAwait(false))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        public static Task<T> ReadAsync<T>(FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            return ReadAsync<T>(file.FullName);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json.
        /// </summary>
        public static void Save<T>(string fileName, T item)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            Ensure.NotNull<object>(item, nameof(item));
            Save(new FileInfo(fileName), item);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml.
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
        /// Saves <paramref name="item"/> as json.
        /// </summary>
        public static Task SaveAsync<T>(string fileName, T item)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            Ensure.NotNull<object>(item, nameof(item));
            return SaveAsync(new FileInfo(fileName), item);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml.
        /// </summary>
        public static async Task SaveAsync<T>(FileInfo file, T item)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull<object>(item, nameof(item));
            using (var stream = ToStream(item))
            {
                await FileHelper.SaveAsync(file, stream).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>.
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
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>.
        /// </summary>
        internal static PooledMemoryStream ToStream<T>(T item)
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