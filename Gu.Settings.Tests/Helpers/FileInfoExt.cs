namespace Gu.Settings.Tests.Helpers
{
    using System.IO;

    public static class FileInfoExt
    {
        public static void VoidCreate(this FileInfo file)
        {
            File.WriteAllText(file.FullName, file.FullName);
        }
    }
}
