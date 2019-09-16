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

            internal abstract void Save<T>(IRepository repository, System.IO.FileInfo file, T item);

            internal virtual System.IO.FileInfo File<T>(IRepository repository, [CallerMemberName] string name = null) => repository.GetFileInfo(name);

            internal System.IO.FileInfo BackupFile<T>(IRepository repository, [CallerMemberName] string name = null)
            {
                return repository.Settings.BackupSettings is IBackupSettings backupSettings
                    ? Core.Backup.BackupFile.CreateFor(this.File<T>(repository, name), backupSettings)
                    : null;
            }
        }

        private class FileInfo : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(file);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file, create);

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.Save(file, item);
        }

        private class FileInfoAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(file).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file, create).Result;

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.SaveAsync(file, item).Wait();
        }

        private class FullFileName : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(file.FullName);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.FullName, create);

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.Save(file.FullName, item);
        }

        private class FullFileNameAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(file.FullName).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.FullName, create).Result;

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.SaveAsync(file.FullName, item).Wait();
        }

        private class FileNameWithExtension : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(file.Name);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.Name, create);

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.Save(file.Name, item);
        }

        private class FileNameWithExtensionAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(file.Name).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.Name, create).Result;

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.SaveAsync(file.Name, item).Wait();
        }

        private class FileNameWithOutExtension : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.Read<T>(Path.GetFileNameWithoutExtension(file.FullName));

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(Path.GetFileNameWithoutExtension(file.FullName), create);

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.Save(Path.GetFileNameWithoutExtension(file.FullName), item);
        }

        private class FileNameWithOutExtensionAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo file) => repository.ReadAsync<T>(Path.GetFileNameWithoutExtension(file.FullName)).Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(Path.GetFileNameWithoutExtension(file.FullName), create).Result;

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.SaveAsync(Path.GetFileNameWithoutExtension(file.FullName), item).Wait();
        }

        private class Generic : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo _) => repository.Read<T>();

            internal override System.IO.FileInfo File<T>(IRepository repository, string name = null) => repository.GetFileInfo(typeof(T).Name);

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(create);

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.Save(item);
        }

        private class GenericAsync : TestCase
        {
            internal override T Read<T>(IRepository repository, System.IO.FileInfo _) => repository.ReadAsync<T>().Result;

            internal override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(create).Result;

            internal override System.IO.FileInfo File<T>(IRepository repository, string name = null) => repository.GetFileInfo(typeof(T).Name);

            internal override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.SaveAsync(item).Wait();
        }
    }
}