namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public abstract partial class RepositoryTests
    {
        public static readonly TestCase[] TestCases =
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

        public abstract class TestCase
        {
            internal abstract T Read<T>(IRepository repository, System.IO.FileInfo file);

            internal abstract T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create);

            internal virtual System.IO.FileInfo File<T>(IRepository repository, [CallerMemberName] string name = null) => repository.GetFileInfo(name);
        }

        private class FileInfo : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(file);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file, create);
        }

        private class FileInfoAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(file).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file, create).Result;
        }

        private class FullFileName : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(file.FullName);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.FullName, create);
        }

        private class FullFileNameAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(file.FullName).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.FullName, create).Result;
        }

        private class FileNameWithExtension : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(file.Name);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.Name, create);
        }

        private class FileNameWithExtensionAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(file.Name).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.Name, create).Result;
        }

        private class FileNameWithOutExtension : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(Path.GetFileNameWithoutExtension(file.FullName));

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(Path.GetFileNameWithoutExtension(file.FullName), create);
        }

        private class FileNameWithOutExtensionAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(Path.GetFileNameWithoutExtension(file.FullName)).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(Path.GetFileNameWithoutExtension(file.FullName), create).Result;
        }

        private class Generic : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo _) => repository.Read<T>();

            internal override System.IO.FileInfo File<T>(IRepository repository, string name = null) => repository.GetFileInfo(typeof(T).Name);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(create);
        }

        private class GenericAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo _) => repository.ReadAsync<T>().Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(create).Result;

            internal override System.IO.FileInfo File<T>(IRepository repository, string name = null) => repository.GetFileInfo(typeof(T).Name);
        }
    }
}