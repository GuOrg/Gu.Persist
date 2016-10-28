namespace Gu.Persist.NewtonsoftJson
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Gu.Persist.Core;

    using Newtonsoft.Json;

    /// <summary>
    /// Helper methods for serializing and deserializing json.
    /// </summary>
    public static class JsonFile
    {
        /// <summary>
        /// Returns the default encoding UTF8.
        /// </summary>
        public static readonly UTF8Encoding DefaultEncoding = new UTF8Encoding(false, true);

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
        /// Serializes to memorystream, then returns the deserialized object
        /// </summary>
        public static T Clone<T>(T item, JsonSerializerSettings settings)
        {
            Ensure.NotNull<object>(item, nameof(item));
            using (var stream = ToStream(item, settings))
            {
                return FromStream<T>(stream, settings);
            }
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(FileInfo file)
        {
            Ensure.Exists(file, nameof(file));
            return FileHelper.Read(file, FromStream<T>);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(FileInfo file, JsonSerializerSettings settings)
        {
            Ensure.Exists(file, nameof(file));
            return FileHelper.Read(file, s => FromStream<T>(s, settings));
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static Task<T> ReadAsync<T>(FileInfo file)
        {
            Ensure.Exists(file, nameof(file));
            return FileHelper.ReadAsync(file, FromStream<T>);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static Task<T> ReadAsync<T>(FileInfo file, JsonSerializerSettings settings)
        {
            Ensure.Exists(file, nameof(file));
            return FileHelper.ReadAsync(file, s => FromStream<T>(s, settings));
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json
        /// </summary>
        public static void Save<T>(FileInfo file, T item)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull<object>(item, nameof(item));
            Save(file, item, null);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json
        /// </summary>
        public static void Save<T>(FileInfo file, T item, JsonSerializerSettings settings)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull<object>(item, nameof(item));
            var serializer = settings != null
                ? JsonSerializer.Create(settings)
                : JsonSerializer.Create();
            using (var writer = new JsonTextWriter(new StreamWriter(file.OpenCreate(), DefaultEncoding, 1024, false)))
            {
                serializer.Serialize(writer, item);
            }
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json
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
        /// Saves <paramref name="item"/> as json
        /// </summary>
        public static Task SaveAsync<T>(FileInfo file, T item, JsonSerializerSettings settings)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull<object>(item, nameof(item));
            using (var stream = ToStream(item, settings))
            {
                return FileHelper.SaveAsync(file, stream);
            }
        }

        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        internal static T FromStream<T>(Stream stream)
        {
            return FromStream<T>(stream, null);
        }

        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        internal static T FromStream<T>(Stream stream, JsonSerializerSettings settings)
        {
            var serializer = settings != null
                ? JsonSerializer.Create(settings)
                : JsonSerializer.Create();
            using (var sr = new StreamReader(stream, DefaultEncoding, true, 1024, true))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>
        /// </summary>
        internal static MemoryStream ToStream<T>(T item)
        {
            return ToStream(item, null);
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>
        /// </summary>
        internal static MemoryStream ToStream<T>(T item, JsonSerializerSettings settings)
        {
            var stream = PooledMemoryStream.Borrow();
            var serializer = settings != null
                ? JsonSerializer.Create(settings)
                : JsonSerializer.Create();
            using (var writer = new JsonTextWriter(new StreamWriter(stream, DefaultEncoding, 1024, true)))
            {
                serializer.Serialize(writer, item);
            }

            stream.Position = 0;
            return stream;
        }
    }
}