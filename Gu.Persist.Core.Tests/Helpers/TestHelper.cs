namespace Gu.Persist.Core.Tests
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class TestHelper
    {
        public static T Read<T>(this FileInfo file)
        {
            var formatter = new BinaryFormatter();
            using var stream = File.OpenRead(file.FullName);
#pragma warning disable CA2300, CA2301 // Do not use insecure deserializer BinaryFormatter
            return (T)formatter.Deserialize(stream);
#pragma warning restore CA2300, CA2301 // Do not use insecure deserializer BinaryFormatter
        }

        public static void Save<T>(this FileInfo file, T o)
            where T : notnull
        {
            var formatter = new BinaryFormatter();

            using var stream = File.OpenWrite(file.FullName);
            formatter.Serialize(stream, o);
        }
    }
}