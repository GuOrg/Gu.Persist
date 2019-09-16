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
            new FileInfo(),
            new FullFileName(),
            new FileNameWithExtension(),
            new FileNameWithOutExtension(),
            new Generic(),
            new FileInfoAsync(),
            new FullFileNameAsync(),
            new FileNameWithExtensionAsync(),
            new FileNameWithOutExtensionAsync(),
            new GenericAsync(),
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
        public void ReadManagesSingleton(ReadCase testCase)
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
        public void ReadOrCreateWhenFileExists(ReadCase testCase)
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
        public void ReadOrCreateWhenFileDoesNotExists(ReadCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var read = testCase.ReadOrCreate(repository, file, () => dummy);
            Assert.AreSame(dummy, read);
        }

        public abstract class ReadCase
        {
            internal abstract T Read<T>(IRepository repository, System.IO.FileInfo file);

            internal abstract T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create);

            internal virtual System.IO.FileInfo File<T>(IRepository repository, [CallerMemberName] string name = null) => repository.GetFileInfo(name);
        }

        private class FileInfo : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(file);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file, create);
        }

        private class FileInfoAsync : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(file).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file, create).Result;
        }

        private class FullFileName : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(file.FullName);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.FullName, create);
        }

        private class FullFileNameAsync : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(file.FullName).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.FullName, create).Result;
        }

        private class FileNameWithExtension : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(file.Name);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.Name, create);
        }

        private class FileNameWithExtensionAsync : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(file.Name).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.Name, create).Result;
        }

        private class FileNameWithOutExtension : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(Path.GetFileNameWithoutExtension(file.FullName));

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(Path.GetFileNameWithoutExtension(file.FullName), create);
        }

        private class FileNameWithOutExtensionAsync : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(Path.GetFileNameWithoutExtension(file.FullName)).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(Path.GetFileNameWithoutExtension(file.FullName), create).Result;
        }

        private class Generic : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo _) => repository.Read<T>();

            internal override System.IO.FileInfo File<T>(IRepository repository, string name = null) => repository.GetFileInfo(typeof(T).Name);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(create);
        }

        private class GenericAsync : ReadCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo _) => repository.ReadAsync<T>().Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(create).Result;

            internal override System.IO.FileInfo File<T>(IRepository repository, string name = null) => repository.GetFileInfo(typeof(T).Name);
        }
    }
}