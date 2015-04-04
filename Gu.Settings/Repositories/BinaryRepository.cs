namespace Gu.Settings
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;

    public static class BinaryRepository
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
        /// <param name="throwIfNotExists">Throws and exception if file is missing when true.
        /// If false it returns default(T)</param>
        /// <returns></returns>
        public static T ReadBinary<T>(string fullFileName, bool throwIfNotExists)
        {
            return FileHelper.Read(fullFileName, FromBinaryStream<T>, throwIfNotExists);
        }

        /// <summary>
        /// Reads an xml file and deserializes the contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullFileName">The filename including path and extension</param>
        /// <param name="throwIfNotExists">Throws and exception if file is missing when true.
        /// If false it returns default(T)</param>
        /// <returns></returns>
        public static Task<T> ReadBinaryAsync<T>(string fullFileName, bool throwIfNotExists)
        {
            return FileHelper.ReadAsync(fullFileName, FromBinaryStream<T>, throwIfNotExists);
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