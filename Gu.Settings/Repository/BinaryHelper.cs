namespace Gu.Settings
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;

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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        public static T ReadBinary<T>(FileInfo file)
        {
            return FileHelper.Read(file, FromStream<T>);
        }

        /// <summary>
        /// Reads an xml file and deserializes the contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The filename including path and extension</param>
        /// <returns></returns>
        public static Task<T> ReadBinaryAsync<T>(FileInfo file)
        {
            return FileHelper.ReadAsync(file, FromStream<T>);
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
            using (var stream = ToStream(o))
            {
                return FileHelper.SaveAsync(file, stream);
            }
        }

        public static void SaveBinary<T>(T o, FileInfo file)
        {
            using (var stream = ToStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}