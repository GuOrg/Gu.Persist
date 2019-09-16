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
            public abstract T Read<T>(IRepository repository, System.IO.FileInfo file, Migration migration = null);

            public abstract T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create);

            public abstract void Save<T>(IRepository repository, System.IO.FileInfo file, T item);

            public abstract bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item);

            public virtual System.IO.FileInfo File<T>(IRepository repository, [CallerMemberName] string name = null) => repository.GetFileInfo(name);

            public System.IO.FileInfo BackupFile<T>(IRepository repository, [CallerMemberName] string name = null)
            {
                return repository.Settings.BackupSettings is IBackupSettings backupSettings
                    ? Core.Backup.BackupFile.CreateFor(this.File<T>(repository, name), backupSettings)
                    : null;
            }
        }

        private class FileInfo : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo file, Migration migration = null) => repository.Read<T>(file);

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file, create);

            public override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.Save(file, item);

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(file, item);
        }

        private class FileInfoAsync : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo file, Migration migration = null) => repository.ReadAsync<T>(file).Result;

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file, create).Result;

            public override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.SaveAsync(file, item).Wait();

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(file, item);
        }

        private class FullFileName : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo file, Migration migration = null) => repository.Read<T>(file.FullName);

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.FullName, create);

            public override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.Save(file.FullName, item);

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(file.FullName, item);
        }

        private class FullFileNameAsync : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo file, Migration migration = null) => repository.ReadAsync<T>(file.FullName).Result;

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.FullName, create).Result;

            public override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.SaveAsync(file.FullName, item).Wait();

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(file.FullName, item);
        }

        private class FileNameWithExtension : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo file, Migration migration = null) => repository.Read<T>(file.Name);

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(file.Name, create);

            public override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.Save(file.Name, item);

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(file.Name, item);
        }

        private class FileNameWithExtensionAsync : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo file, Migration migration = null) => repository.ReadAsync<T>(file.Name).Result;

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(file.Name, create).Result;

            public override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.SaveAsync(file.Name, item).Wait();

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(file.Name, item);
        }

        private class FileNameWithOutExtension : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo file, Migration migration = null) => repository.Read<T>(Path.GetFileNameWithoutExtension(file.FullName));

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreate<T>(Path.GetFileNameWithoutExtension(file.FullName), create);

            public override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.Save(Path.GetFileNameWithoutExtension(file.FullName), item);

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(Path.GetFileNameWithoutExtension(file.FullName), item);
        }

        private class FileNameWithOutExtensionAsync : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo file, Migration migration = null) => repository.ReadAsync<T>(Path.GetFileNameWithoutExtension(file.FullName)).Result;

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo file, Func<T> create) => repository.ReadOrCreateAsync<T>(Path.GetFileNameWithoutExtension(file.FullName), create).Result;

            public override void Save<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.SaveAsync(Path.GetFileNameWithoutExtension(file.FullName), item).Wait();

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(Path.GetFileNameWithoutExtension(file.FullName), item);
        }

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        private class Generic : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo _, Migration migration = null) => repository.Read<T>(migration);

            public override System.IO.FileInfo File<T>(IRepository repository, string _ = null) => repository.GetFileInfo(typeof(T).Name);

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo _, Func<T> create) => repository.ReadOrCreate<T>(create);

            public override void Save<T>(IRepository repository, System.IO.FileInfo _, T item) => repository.Save(item);

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(item);
        }

        private class GenericAsync : TestCase
        {
            public override T Read<T>(IRepository repository, System.IO.FileInfo _, Migration migration = null) => repository.ReadAsync<T>().Result;

            public override T ReadOrCreate<T>(IRepository repository, System.IO.FileInfo _, Func<T> create) => repository.ReadOrCreateAsync<T>(create).Result;

            public override System.IO.FileInfo File<T>(IRepository repository, string _ = null) => repository.GetFileInfo(typeof(T).Name);

            public override void Save<T>(IRepository repository, System.IO.FileInfo _, T item) => repository.SaveAsync(item).Wait();

            public override bool IsDirty<T>(IRepository repository, System.IO.FileInfo file, T item) => repository.IsDirty(item);
        }
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    }
}