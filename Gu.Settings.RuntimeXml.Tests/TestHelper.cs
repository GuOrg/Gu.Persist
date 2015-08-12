namespace Gu.Settings.RuntimeXml.Tests
{
    using System.IO;

    using Gu.Settings.Core;

    public static class TestHelper
    {
        public static T Read<T>(FileInfo file)
        {
            return FileHelper.Read(file, XmlHelper.FromStream<T>);
        }

        public static void Save<T>(T o, FileInfo file)
        {
            using (var stream = XmlHelper.ToStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}