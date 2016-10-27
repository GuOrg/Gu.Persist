namespace Gu.Settings.RuntimeBinary
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;

    using Gu.Settings.Core;

    /// <summary>
    /// Helper methods for reading  json.
    /// </summary>
    public static class BinaryHelper
    {
        /// <summary>
        /// Deserialize the contents of <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        public static T FromStream<T>(Stream stream)
        {
            var formatter = new BinaryFormatter();
            var setting = (T)formatter.Deserialize(stream);
            return setting;
        }

        /// <summary>
        /// Serialize <paramref name="item"/> to a <see cref="MemoryStream"/>
        /// </summary>
        public static MemoryStream ToStream<T>(T item)
        {
            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            formatter.Serialize(ms, item);
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
        /// Save the binary representation of <paramref name="item"/>.
        /// </summary>
        public static Task SaveAsync<T>(T item, FileInfo file)
        {
            using (var stream = ToStream(item))
            {
                return FileHelper.SaveAsync(file, stream);
            }
        }

        /// <summary>
        /// Save the binary representation of <paramref name="item"/>.
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