namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    internal static class XmlSerializerRepository
    {
        internal static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new ConcurrentDictionary<Type, XmlSerializer>();

        internal static T FromXmlStream<T>(Stream stream)
        {
            var serializer = Serializers.GetOrAdd(typeof(T), x => new XmlSerializer(typeof(T)));
            var setting = (T)serializer.Deserialize(stream);
            return setting;
        }

        internal static MemoryStream ToXmlStream<T>(T o)
        {
            var serializer = Serializers.GetOrAdd(o.GetType(), x => new XmlSerializer(o.GetType()));
            var ms = new MemoryStream();
            serializer.Serialize(ms, o);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullFileName">The filename including path and extension</param>
        /// <param name="throwIfNotExists">Throws and exception if file is missing when true.
        /// If false it returns default(T)</param>
        /// <returns></returns>
        internal static T ReadXml<T>(string fullFileName, bool throwIfNotExists)
        {
            return FileHelper.Read(fullFileName, FromXmlStream<T>, throwIfNotExists);
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullFileName">The filename including path and extension</param>
        /// <param name="throwIfNotExists">Throws and exception if file is missing when true.
        /// If false it returns default(T)</param>
        /// <returns></returns>
        internal static Task<T> ReadXmlAsync<T>(string fullFileName, bool throwIfNotExists)
        {
            return FileHelper.ReadAsync(fullFileName, FromXmlStream<T>, throwIfNotExists);
        }

        /// <summary>
        /// Saves as xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="fullFileName">The filename including path and extension</param>
        /// <returns></returns>
        internal static Task SaveXmlAsync<T>(T o, string fullFileName)
        {
            return FileHelper.SaveAsync(o, fullFileName, ToXmlStream);
        }

        internal static void SaveXml<T>(T o, string fullFileName)
        {
            FileHelper.Save(o, fullFileName, ToXmlStream);
        }
    }
}
