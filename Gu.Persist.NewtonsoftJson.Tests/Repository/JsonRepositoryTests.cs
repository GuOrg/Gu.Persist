namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using System;
    using System.IO;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.Core.Tests.Repositories;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    public abstract class JsonRepositoryTests : RepositoryTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void ReadWhenTrivialMigration(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var read = testCase.Read<DummySerializable>(repository, file, new JsonMigration(x => Migrate(x)));
            Assert.AreEqual(2, read.Value);
            Assert.AreEqual(2, testCase.Read<DummySerializable>(repository, file).Value);

            static JObject Migrate(JObject jObject)
            {
                jObject["Value"] = 2;
                return jObject;
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadWhenMigration(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            File.WriteAllText(file.FullName, "{ \"Old\": 1 }");
            var read = testCase.Read<DummySerializable>(repository, file, new JsonMigration(x => Migrate(x)));
            Assert.AreEqual(2, read.Value);
            Assert.AreEqual(2, testCase.Read<DummySerializable>(repository, file).Value);

            static JObject Migrate(JObject jObject)
            {
                if (jObject.TryGetValue("Old", StringComparison.Ordinal, out _))
                {
                    jObject.Remove("Old");
                    jObject["Value"] = 2;
                }

                return jObject;
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenMigration(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            File.WriteAllText(file.FullName, "{ \"Old\": 1 }");
            var read = testCase.ReadOrCreate(repository, file, () => new DummySerializable(-1), new JsonMigration(x => Migrate(x)));
            Assert.AreEqual(2, read.Value);
            Assert.AreEqual(2, testCase.Read<DummySerializable>(repository, file).Value);

            static JObject Migrate(JObject jObject)
            {
                if (jObject.TryGetValue("Old", StringComparison.Ordinal, out _))
                {
                    jObject.Remove("Old");
                    jObject["Value"] = 2;
                }

                return jObject;
            }
        }

        protected override void Save<T>(FileInfo file, T item)
        {
            JsonFile.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return JsonFile.Read<T>(file);
        }
    }
}