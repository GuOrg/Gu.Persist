namespace Gu.Settings.Tests
{
    using System.IO;

    public static class TestFileInfoExt
    {
        public static void VoidCreate(this FileInfo file)
        {
            File.WriteAllText(file.FullName, file.FullName);
        }

        public static void WriteAllText(this FileInfo file, string text)
        {
            File.WriteAllText(file.FullName, text);
        }

        public static void WriteXml<T>(this FileInfo file, T item)
        {
            XmlHelper.Save(item, file);
        }

        public static string ReadAllText(this FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }

        public static T ReadXml<T>(this FileInfo file)
        {
            return XmlHelper.Read<T>(file);
        }
    }
}
