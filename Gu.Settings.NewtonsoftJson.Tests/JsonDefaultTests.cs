﻿namespace Gu.Settings.NewtonsoftJson.Tests
{
    using System.IO;
    using Core.Tests;
    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests.Repositories;
    using Gu.Settings.NewtonsoftJson;
    using NUnit.Framework;

    public class JsonDefaultTests : RepositoryTests
    {
        [Test]
        public void SaveAndCheckJson()
        {
            var fileInfo = this.Directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            var value = new DummySerializable
            {
                Value = 1
            };
            this.Repository.Save(value, fileInfo);
            var json = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("{\r\n  \"Value\": 1\r\n}", json);
            File.Delete(fileInfo.FullName);
        }

        protected override IRepository Create()
        {
            var settings = JsonRepositorySettings.DefaultFor(this.Directory);
            return new JsonRepository(settings);
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