#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using NUnit.Framework;

    public abstract partial class RepositoryTests
    {
        public static readonly ReadCase[] TestCases =
        {
            new ReadFileInfo(),
            new ReadFullName(),
            new ReadNameWithExtension(),
            new ReadNameWithOutExtension(),
            new ReadGeneric(),
            new ReadAsyncFileInfo(),
            new ReadAsyncFullName(),
            new ReadAsyncNameWithExtension(),
            new ReadAsyncNameWithOutExtension(),
            new ReadAsyncGeneric(),
        };

        [TestCaseSource(nameof(TestCases))]
        public void Read(ReadCase testCase)
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
        public void ReadCaches(ReadCase testCase)
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

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateFileInfo(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();

            if (exists)
            {
                this.Save(file, dummy);
            }

            var read = repository.ReadOrCreate(file, () => dummy);
            AssertFile.Exists(true, file);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateFullName(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();

            if (exists)
            {
                this.Save(file, dummy);
            }

            var read = repository.ReadOrCreate(file.FullName, () => dummy);
            AssertFile.Exists(true, file);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateName(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();

            if (exists)
            {
                this.Save(file, dummy);
            }

            var read = repository.ReadOrCreate(file.Name, () => dummy);
            AssertFile.Exists(true, file);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateGeneric(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);

            if (exists)
            {
                this.Save(file, dummy);
            }

            var read = repository.ReadOrCreate(() => dummy);
            AssertFile.Exists(true, file);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        public abstract class ReadCase
        {
            internal abstract T Read<T>(IRepository repository, FileInfo file);

            internal abstract T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create);

            internal virtual FileInfo File<T>(IRepository repository, [CallerMemberName] string name = null) => repository.GetFileInfo(name);
        }

        private class ReadFileInfo : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo file) => repository.Read<T>(file);

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file, create);
        }

        private class ReadFullName : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo file) => repository.Read<T>(file.FullName);

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.FullName, create);
        }

        private class ReadNameWithExtension : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo file) => repository.Read<T>(file.Name);

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.Name, create);
        }

        private class ReadNameWithOutExtension : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo file) => repository.Read<T>(Path.GetFileNameWithoutExtension(file.FullName));

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(Path.GetFileNameWithoutExtension(file.FullName), create);
        }

        private class ReadGeneric : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo _) => repository.Read<T>();

            internal override FileInfo File<T>(IRepository repository, string name = null) => repository.GetFileInfo(typeof(T).Name);

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(create);
        }

        private class ReadAsyncFileInfo : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo file) => repository.ReadAsync<T>(file).Result;

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file, create).Result;
        }

        private class ReadAsyncFullName : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo file) => repository.ReadAsync<T>(file.FullName).Result;

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.FullName, create).Result;
        }

        private class ReadAsyncNameWithExtension : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo file) => repository.ReadAsync<T>(file.Name).Result;

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.Name, create).Result;
        }

        private class ReadAsyncNameWithOutExtension : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo file) => repository.ReadAsync<T>(Path.GetFileNameWithoutExtension(file.FullName)).Result;

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(Path.GetFileNameWithoutExtension(file.FullName), create).Result;
        }

        private class ReadAsyncGeneric : ReadCase
        {
            internal override T Read<T>(IRepository repository, FileInfo _) => repository.ReadAsync<T>().Result;

            internal override T ReadOrCreate<T>(IRepository repository, FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(create).Result;

            internal override FileInfo File<T>(IRepository repository, string name = null) => repository.GetFileInfo(typeof(T).Name);
        }
    }
}