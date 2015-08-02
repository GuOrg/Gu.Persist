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

        public static string ReadAllText(this FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }
    }
}
