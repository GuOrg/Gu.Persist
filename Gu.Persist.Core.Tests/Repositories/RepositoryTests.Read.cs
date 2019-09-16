#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using NUnit.Framework;

    public abstract partial class RepositoryTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void Read(TestCase testCase)
        {
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var dummy = new DummySerializable(1);
            this.Save(file, dummy);
            var read = testCase.Read<DummySerializable>(repository, file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadManagesSingleton(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var read1 = repository.Read<DummySerializable>(file);
            var read2 = repository.Read<DummySerializable>(file);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenFileExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var read = testCase.ReadOrCreate<DummySerializable>(repository, file, () => throw new AssertionException("Should not get here."));
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenFileDoesNotExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var read = testCase.ReadOrCreate(repository, file, () => dummy);
            Assert.AreSame(dummy, read);
        }
    }
}