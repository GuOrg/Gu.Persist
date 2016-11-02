namespace Gu.Persist.Yaml
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Gu.Persist.Core;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Helper class for serializing and deserializing using <see cref="YamlDotNet.Serialization.Serializer"/>
    /// </summary>
    public static class YamlFile
    {
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
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(string fileName)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            return FileHelper.Read(new FileInfo(fileName), FromStream<T>);
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
        public static Task<T> ReadAsync<T>(string fileName)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            return FileHelper.ReadAsync(new FileInfo(fileName), FromStream<T>);
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
        /// Saves <paramref name="item"/> as json
        /// </summary>
        public static void Save<T>(string fileName, T item)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            Ensure.NotNull<object>(item, nameof(item));
            Save(new FileInfo(fileName), item);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml
        /// </summary>
        public static void Save<T>(FileInfo file, T item)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull<object>(item, nameof(item));
            var serializer = CreateSerializer();
            using (var stream = file.OpenCreate())
            {
                using (var writer = new StreamWriter(stream))
                {
                    serializer.Serialize(writer, item);
                }
            }
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json
        /// </summary>
        public static Task SaveAsync<T>(string fileName, T item)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            Ensure.NotNull<object>(item, nameof(item));
            return SaveAsync(new FileInfo(fileName), item);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml
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
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        internal static T FromStream<T>(Stream stream)
        {
            var serializer = new Deserializer();
            T setting;
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                setting = (T)serializer.Deserialize<T>(reader);
            }

            return setting;
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>
        /// </summary>
        internal static PooledMemoryStream ToStream<T>(T item)
        {
            var serializer = CreateSerializer();
            var stream = PooledMemoryStream.Borrow();
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                serializer.Serialize(writer, item);
            }

            stream.Flush();
            stream.Position = 0;
            return stream;
        }

        internal static Serializer CreateSerializer()
        {
            return new Serializer();
        }
    }
}