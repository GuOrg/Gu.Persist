﻿namespace Gu.Persist.Git.Tests
{
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.NewtonsoftJson;

    public static class TestHelper
    {
        public static T Read<T>(FileInfo file)
        {
            return FileHelper.Read(file, JsonHelper.FromStream<T>);
        }

        public static void Save<T>(T o, FileInfo file)
        {
            using (var stream = JsonHelper.ToStream(o))
            {
                FileHelper.Save(file, stream);
            }
        }
    }
}