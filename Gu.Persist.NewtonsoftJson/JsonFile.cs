namespace Gu.Persist.NewtonsoftJson
{
    using System;
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
        public static readonly UTF8Encoding DefaultEncoding = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        /// <summary>
        /// Serializes to MemoryStream, then returns the deserialized object.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The <typeparamref name="T"/>.</param>
        /// <returns>The deep clone.</returns>
        public static T Clone<T>(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using var stream = ToStream(item);
            return FromStream<T>(stream)!;
        }

        /// <summary>
        /// Serializes to MemoryStream, then returns the deserialized object.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The <typeparamref name="T"/>.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The deep clone.</returns>
        public static T Clone<T>(T item, JsonSerializerSettings? settings)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            using var stream = ToStream(item, settings);
            return FromStream<T>(stream, settings)!;
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents of the file to.</typeparam>
        /// <param name="fileName">The full name of the file.</param>
        /// <returns>The deserialized content.</returns>
        public static T Read<T>(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using var stream = File.OpenRead(fileName);
            return FromStream<T>(stream);
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents of the file to.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <returns>The deserialized content.</returns>
        public static T Read<T>(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using var stream = File.OpenRead(file.FullName);
            return FromStream<T>(stream);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents of the file to.</typeparam>
        /// <param name="fileName">The full name of the file.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The deserialized content.</returns>
        public static T Read<T>(string fileName, JsonSerializerSettings? settings)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using var stream = File.OpenRead(fileName);
            return FromStream<T>(stream, settings);
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents of the file to.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The deserialized content.</returns>
        public static T Read<T>(FileInfo file, JsonSerializerSettings? settings)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return Read<T>(file.FullName, settings);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents of the file to.</typeparam>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A <see cref="Task"/> with the deserialized content of the file.</returns>
        public static async Task<T> ReadAsync<T>(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using var stream = await FileHelper.ReadAsync(fileName).ConfigureAwait(false);
            return FromStream<T>(stream);
        }

        /// <summary>
        /// Read the file and deserialize the contents to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents of the file to.</typeparam>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>A <see cref="Task"/> with the deserialized content of the file.</returns>
        public static async Task<T> ReadAsync<T>(string fileName, JsonSerializerSettings? settings)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using var stream = await FileHelper.ReadAsync(fileName).ConfigureAwait(false);
            return FromStream<T>(stream, settings);
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents of the file to.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <returns>A <see cref="Task"/> with the deserialized content of the file.</returns>
        public static Task<T> ReadAsync<T>(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return ReadAsync<T>(file.FullName);
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents of the file to.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>A <see cref="Task"/> with the deserialized content of the file.</returns>
        public static Task<T> ReadAsync<T>(FileInfo file, JsonSerializerSettings? settings)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return ReadAsync<T>(file.FullName, settings);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fileName">The file name.</param>
        /// <param name="item">The <typeparamref name="T"/>.</param>
        public static void Save<T>(string fileName, T item)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Save(new FileInfo(fileName), item);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The instance to serialize.</param>
        public static void Save<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Save(file, item, null);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fileName">The file name.</param>
        /// <param name="item">The <typeparamref name="T"/>.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        public static void Save<T>(string fileName, T item, JsonSerializerSettings settings)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Save(new FileInfo(fileName), item, settings);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The instance to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        public static void Save<T>(FileInfo file, T item, JsonSerializerSettings? settings)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var serializer = settings != null
                ? JsonSerializer.Create(settings)
                : JsonSerializer.Create();
            using var streamWriter = new StreamWriter(file.OpenCreate(), DefaultEncoding, bufferSize: 1024, leaveOpen: false);
            using var writer = new JsonTextWriter(streamWriter);
            serializer.Serialize(writer, item);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fileName">The full name of the file.</param>
        /// <param name="item">The instance to serialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous save operation.</returns>
        public static Task SaveAsync<T>(string fileName, T item)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return SaveAsync(new FileInfo(fileName), item);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The instance to serialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous save operation.</returns>
        public static async Task SaveAsync<T>(FileInfo file, T item)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using var stream = ToStream(item);
            await FileHelper.SaveAsync(file, stream).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as json.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="fileName">The full name of the file.</param>
        /// <param name="item">The instance to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous save operation.</returns>
        public static Task SaveAsync<T>(string fileName, T item, JsonSerializerSettings? settings)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return SaveAsync(new FileInfo(fileName), item, settings);
        }

        /// <summary>
        /// Saves <paramref name="item"/> as xml.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="item">The instance to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous save operation.</returns>
        public static async Task SaveAsync<T>(FileInfo file, T item, JsonSerializerSettings? settings)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using var stream = ToStream(item, settings);
            await FileHelper.SaveAsync(file, stream).ConfigureAwait(false);
        }

        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents to.</typeparam>
        /// <param name="stream">The <see cref="Stream"/>.</param>
        /// <returns>The deserialized contents.</returns>
        internal static T FromStream<T>(Stream stream)
        {
            return FromStream<T>(stream, null);
        }

        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents to.</typeparam>
        /// <param name="stream">The <see cref="Stream"/>.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The deserialized contents.</returns>
        internal static T FromStream<T>(Stream stream, JsonSerializerSettings? settings)
        {
            var serializer = settings != null
                ? JsonSerializer.Create(settings)
                : JsonSerializer.Create();
            using var reader = new StreamReader(stream, DefaultEncoding, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            using var jsonTextReader = new JsonTextReader(reader);
            return serializer.Deserialize<T>(jsonTextReader) ?? throw new InvalidOperationException("The content of the stream deserialized to null.");
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="PooledMemoryStream"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The instance to serialize.</param>
        /// <returns>The <see cref="PooledMemoryStream"/>.</returns>
        internal static PooledMemoryStream ToStream<T>(T item)
        {
            return ToStream(item, null);
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="PooledMemoryStream"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The instance to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The <see cref="PooledMemoryStream"/>.</returns>
        internal static PooledMemoryStream ToStream<T>(T item, JsonSerializerSettings? settings)
        {
            var stream = PooledMemoryStream.Borrow();
            var serializer = settings != null
                ? JsonSerializer.Create(settings)
                : JsonSerializer.Create();
            using (var streamWriter = new StreamWriter(stream, DefaultEncoding, bufferSize: 1024, leaveOpen: true))
            {
                using var writer = new JsonTextWriter(streamWriter);
                serializer.Serialize(writer, item);
            }

            stream.Position = 0;
            return stream;
        }
    }
}