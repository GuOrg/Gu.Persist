﻿namespace Gu.Settings.Json.Tests
{
    using System.IO;

    using Gu.Settings.Tests;
    using Gu.Settings.Tests.Repositories;

    using NUnit.Framework;

    public class JsonDefault : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, RepoSettingFile);
        }

        protected override IRepository Create(RepositorySettings settings)
        {
            return new JsonRepository();
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            JsonHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return JsonHelper.Read<T>(file);
        }
    }
}