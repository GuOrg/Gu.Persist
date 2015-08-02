namespace Gu.Settings.Tests
{
    using System.IO;

    public static class TestHelper
    {
        public static T Read<T>(this FileInfo file)
        {
            return FileHelper.Read(file, BinaryHelper.FromStream<T>);
        }

        public static void Save<T>(this FileInfo file, T o)
        {
            using (var stream = BinaryHelper.ToStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}