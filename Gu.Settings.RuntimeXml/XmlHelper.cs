namespace Gu.Settings.RuntimeXml
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Runtime.Serialization;

    public static class XmlHelper
    {
        internal static readonly ConcurrentDictionary<Type, DataContractSerializer> Serializers = new ConcurrentDictionary<Type, DataContractSerializer>();

        public static T FromStream<T>(Stream stream)
        {
            var serializer = Serializers.GetOrAdd(typeof(T), x => new DataContractSerializer(typeof(T)));
            lock (serializer)
            {
                var setting = (T)serializer.ReadObject(stream);
                return setting;
            }
        }

        public static MemoryStream ToStream<T>(T o)
        {
            var ms = new MemoryStream();
            var serializer = Serializers.GetOrAdd(o.GetType(), x => new DataContractSerializer(o.GetType()));
            lock (serializer)
            {
                serializer.WriteObject(ms, o);
            }

            ms.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}