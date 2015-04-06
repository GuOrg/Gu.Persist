namespace Gu.Settings
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;

    public static class BinaryHelper
    {
        public static T FromBinaryStream<T>(Stream stream)
        {
            var formatter = new BinaryFormatter();
            var setting = (T)formatter.Deserialize(stream);
            return setting;
        }

        public static MemoryStream ToBinaryStream<T>(T o)
        {
            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            formatter.Serialize(ms, o);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        public static T ReadBinary<T>(FileInfo file)
        {
            return FileHelper.Read(file, FromBinaryStream<T>);
        }

        /// <summary>
        /// Reads an xml file and deserializes the contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The filename including path and extension</param>
        /// <returns></returns>
        public static Task<T> ReadBinaryAsync<T>(FileInfo file)
        {
            return FileHelper.ReadAsync(file, FromBinaryStream<T>);
        }

        /// <summary>
        /// Saves as xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="file">The filename including path and extension</param>
        /// <returns></returns>
        public static Task SaveBinaryAsync<T>(T o, FileInfo file)
        {
            using (var stream = ToBinaryStream(o))
            {
                return FileHelper.SaveAsync(file, stream);
            }
        }

        public static void SaveBinary<T>(T o, FileInfo file)
        {
            using (var stream = ToBinaryStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}