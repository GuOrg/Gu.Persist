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
        public static T Read<T>(string fileName)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            using (var stream = File.OpenRead(fileName))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(FileInfo file)
        {
            Ensure.Exists(file, nameof(file));
            using (var stream = File.OpenRead(file.FullName))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(string fileName, JsonSerializerSettings settings)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            using (var stream = File.OpenRead(fileName))
            {
                return FromStream<T>(stream, settings);
            }
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(FileInfo file, JsonSerializerSettings settings)
        {
            Ensure.Exists(file, nameof(file));
            return Read<T>(file.FullName, settings);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
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
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static Task<T> ReadAsync<T>(FileInfo file)
        {
            Ensure.Exists(file, nameof(file));
            return ReadAsync<T>(file.FullName);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static async Task<T> ReadAsync<T>(string fileName, JsonSerializerSettings settings)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            using (var stream = await FileHelper.ReadAsync(fileName).ConfigureAwait(false))
            {
                return FromStream<T>(stream, settings);
            }
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>
        /// </summary>
        public static Task<T> ReadAsync<T>(FileInfo file, JsonSerializerSettings settings)
        {
            Ensure.Exists(file, nameof(file));
            return ReadAsync<T>(file.FullName, settings);
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
        public static void Save<T>(string fileName, T item, JsonSerializerSettings settings)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            Ensure.NotNull<object>(item, nameof(item));
            Save(new FileInfo(fileName), item, settings);
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
        public static Task SaveAsync<T>(string fileName, T item)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            Ensure.NotNull<object>(item, nameof(item));
            return SaveAsync(new FileInfo(fileName), item);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json
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
        /// Saves <paramref name="item"/> as json
        /// </summary>
        public static Task SaveAsync<T>(string fileName, T item, JsonSerializerSettings settings)
        {
            Ensure.NotNull(fileName, nameof(fileName));
            Ensure.NotNull<object>(item, nameof(item));
            return SaveAsync(new FileInfo(fileName), item, settings);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json
        /// </summary>
        public static async Task SaveAsync<T>(FileInfo file, T item, JsonSerializerSettings settings)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull<object>(item, nameof(item));
            using (var stream = ToStream(item, settings))
            {
                await FileHelper.SaveAsync(file, stream).ConfigureAwait(false);
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
            using (var reader = new StreamReader(stream, DefaultEncoding, true, 1024, true))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>
        /// </summary>
        internal static PooledMemoryStream ToStream<T>(T item)
        {
            return ToStream(item, null);
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>
        /// </summary>
        internal static PooledMemoryStream ToStream<T>(T item, JsonSerializerSettings settings)
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