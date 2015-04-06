namespace Gu.Settings.Repositories
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
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static T ReadBinary<T>(string fullFileName)
        {
            return FileHelper.Read(fullFileName, FromBinaryStream<T>);
        }

        /// <summary>
        /// Reads an xml file and deserializes the contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullFileName">The filename including path and extension</param>
        /// <returns></returns>
        public static Task<T> ReadBinaryAsync<T>(string fullFileName)
        {
            return FileHelper.ReadAsync(fullFileName, FromBinaryStream<T>);
        }

        /// <summary>
        /// Saves as xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="fullFileName">The filename including path and extension</param>
        /// <returns></returns>
        public static Task SaveBinaryAsync<T>(T o, string fullFileName)
        {
            return FileHelper.SaveAsync(o, fullFileName, ToBinaryStream);
        }

        public static void SaveBinary<T>(T o, string fullFileName)
        {
            FileHelper.Save(o, fullFileName, ToBinaryStream);
        }
    }
}