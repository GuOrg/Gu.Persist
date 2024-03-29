﻿namespace Gu.Persist.SystemXml
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    using Gu.Persist.Core;

    /// <summary>
    /// Helper methods for serializing and deserializing xml.
    /// </summary>
    public static class XmlFile
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new();

        /// <summary>
        /// Serializes to <see cref="MemoryStream"/>, then returns the deserialized object.
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
            return FromStream<T>(stream);
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

            return Read<T>(file.FullName);
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

            var serializer = Serializers.GetOrAdd(item.GetType(), x => new XmlSerializer(item.GetType()));
            using var stream = file.OpenCreate();
            lock (serializer)
            {
                serializer.Serialize(stream, item);
            }
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
            await file.SaveAsync(stream).ConfigureAwait(false);
        }

        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the contents to.</typeparam>
        /// <param name="stream">The <see cref="Stream"/>.</param>
        /// <returns>The deserialized contents.</returns>
        internal static T FromStream<T>(Stream stream)
        {
            var serializer = Serializers.GetOrAdd(typeof(T), x => new XmlSerializer(x));
            lock (serializer)
            {
                using var reader = XmlReader.Create(stream);
                var setting = (T)serializer.Deserialize(reader);
                return setting;
            }
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="PooledMemoryStream"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The instance to serialize.</param>
        /// <returns>The <see cref="PooledMemoryStream"/>.</returns>
        internal static PooledMemoryStream ToStream<T>(T item)
        {
            var ms = PooledMemoryStream.Borrow();
            var serializer = Serializers.GetOrAdd(item?.GetType() ?? typeof(T), x => new XmlSerializer(x));
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
            return Serializers.GetOrAdd(item.GetType(), x => new XmlSerializer(x));
        }
    }
}
