namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    internal static class XmlHelper
    {
        internal static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new ConcurrentDictionary<Type, XmlSerializer>();

        internal static T FromStream<T>(Stream stream)
        {
            var serializer = Serializers.GetOrAdd(typeof(T), x => new XmlSerializer(typeof(T)));
            var setting = (T)serializer.Deserialize(stream);
            return setting;
        }

        internal static MemoryStream ToStream<T>(T o)
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
        /// <param name="file">The filename including path and extension</param>
        /// <returns></returns>
        internal static T ReadXml<T>(FileInfo file)
        {
            return FileHelper.Read(file, FromStream<T>);
        }

        /// <summary>
        /// Reads an xml file and deserialize the contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The filename including path and extension</param>
        /// <returns></returns>
        internal static Task<T> ReadXmlAsync<T>(FileInfo file)
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
        internal static Task SaveXmlAsync<T>(T o, FileInfo file)
        {
            using (var stream = ToStream(o))
            {
                return FileHelper.SaveAsync(file, stream);
            }
        }

        internal static void SaveXml<T>(T o, FileInfo file)
        {
            using (var stream = ToStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}
