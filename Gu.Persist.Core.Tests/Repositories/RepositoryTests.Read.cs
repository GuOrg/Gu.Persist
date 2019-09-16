#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Repositories
{
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
            var read1 = testCase.Read<DummySerializable>(repository, file);
            var read2 = testCase.Read<DummySerializable>(repository, file);
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

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenFileExistsManagesSingleton(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var read1 = testCase.ReadOrCreate<DummySerializable>(repository, file, () => throw new AssertionException("Should not get here."));
            var read2 = testCase.ReadOrCreate<DummySerializable>(repository, file, () => throw new AssertionException("Should not get here."));
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
        public void ReadOrCreateWhenFileDoesNotExistsManagesSingleton(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var read1 = testCase.ReadOrCreate<DummySerializable>(repository, file, () => dummy);
            var read2 = testCase.ReadOrCreate<DummySerializable>(repository, file, () => dummy);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }
    }
}