namespace Gu.Persist.SystemXml.Tests
{
    using System.IO;

    using Gu.Persist.Core;

    public static class XmlTestHelper
    {
        public static T Read<T>(this FileInfo file)
        {
            return FileHelper.Read(file, XmlHelper.FromStream<T>);
        }

        public static void Save<T>(this FileInfo file, T o)
        {
            using (var stream = XmlHelper.ToStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}
