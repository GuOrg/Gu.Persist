namespace Gu.Persist.NewtonsoftJson.Tests
{
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.NewtonsoftJson;

    public static class JsonTestHelper
    {
        public static T Read<T>(this FileInfo file)
        {
            return FileHelper.Read(file, JsonHelper.FromStream<T>);
        }

        public static void Save<T>(this FileInfo file, T o)
        {
            using (var stream = JsonHelper.ToStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}