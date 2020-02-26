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
            return (T)formatter.Deserialize(stream);
        }

        public static void Save<T>(this FileInfo file, T o)
        {
            var formatter = new BinaryFormatter();

            using var stream = File.OpenWrite(file.FullName);
            formatter.Serialize(stream, o);
        }
    }
}