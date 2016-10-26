namespace Gu.Settings.NewtonsoftJson
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Gu.Settings.Core;

    using Newtonsoft.Json;

    public static class JsonHelper
    {
        public static readonly UTF8Encoding DefaultEncoding = new UTF8Encoding(false, true);


        public static T FromStream<T>(Stream stream)
        {
            return FromStream<T>(stream, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static T FromStream<T>(Stream stream, JsonSerializerSettings settings)
        {
            var serializer = settings != null
                ? JsonSerializer.Create(settings)
                : JsonSerializer.Create();
            using (var sr = new StreamReader(stream, DefaultEncoding, true, 1024, true))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        public static MemoryStream ToStream<T>(T item)
        {
            return ToStream(item, null);
        }

        public static MemoryStream ToStream<T>(T item, JsonSerializerSettings settings)
        {
            var stream = new MemoryStream();
            var serializer = settings != null
                ? JsonSerializer.Create(settings)
                : JsonSerializer.Create();
            using (var writer = new JsonTextWriter(new StreamWriter(stream, DefaultEncoding, 1024, true)))
            {
                serializer.Serialize(writer, item);
            }

            stream.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Serializes to memorystream, then returns the deserialized object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T Clone<T>(T item)
        {
            using (var stream = ToStream(item))
            {
                return FromStream<T>(stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        public static T Read<T>(FileInfo file)
        {
            return FileHelper.Read(file, FromStream<T>);
        }

        /// <summary>
        /// Reads an xml file and deserializes the contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The filename including path and extension</param>
        /// <returns></returns>
        public static Task<T> ReadAsync<T>(FileInfo file)
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
        public static Task SaveAsync<T>(T o, FileInfo file)
        {
            using (var stream = ToStream(o))
            {
                return FileHelper.SaveAsync(file, stream);
            }
        }

        public static void Save<T>(T o, FileInfo file)
        {
            using (var stream = ToStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}