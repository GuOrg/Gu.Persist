﻿namespace Gu.Settings.NewtonsoftJson.Tests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests;
    using Gu.Settings.Core.Tests.Repositories;
    using Gu.Settings.NewtonsoftJson;

    using NUnit.Framework;

    public class JsonNullSettings : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, RepoSettingFile);
        }

        protected override IRepository Create()
        {
            return new JsonRepository();
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            TestHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return TestHelper.Read<T>(file);
        }
    }
}
