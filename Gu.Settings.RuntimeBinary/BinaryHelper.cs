namespace Gu.Settings.RuntimeBinary
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;

    using Gu.Settings.Core;

    public static class BinaryHelper
    {
        public static T FromStream<T>(Stream stream)
        {
            var formatter = new BinaryFormatter();
            var setting = (T)formatter.Deserialize(stream);
            return setting;
        }

        public static MemoryStream ToStream<T>(T o)
        {
            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            formatter.Serialize(ms, o);
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
        /// Reads an xml file and deserializes the contents
        /// </summary>
        /// <param name="file">The filename including path and extension</param>
        public static Task<T> ReadAsync<T>(FileInfo file)
        {
            return FileHelper.ReadAsync(file, FromStream<T>);
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
    }
}