namespace Gu.Persist.Git.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.NewtonsoftJson;

    using NUnit.Framework;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public static class GitBackuperTests
    {
        private static readonly DirectoryInfo Directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + nameof(GitBackuperTests));
        private static DummySerializable? dummy;
        private static SingletonRepository? repository;
        private static LockedFile? lockFile;

        [OneTimeSetUp]
        public static async Task OneTimeSetup()
        {
            var lockFileInfo = Directories.TempDirectory.CreateFileInfoInDirectory("test.lock");
            lockFile?.DisposeAndDeleteFile();
            lockFile = await LockedFile.CreateAsync(lockFileInfo, TimeSpan.FromSeconds(60))
                .ConfigureAwait(false);
        }

        [SetUp]
        public static void SetUp()
        {
            if (Directory.Exists)
            {
                DeleteRepositoryDirectory(Directory.FullName);
            }

            Directory.Create();
            var settings = new RepositorySettings(
                directory: Directory.FullName,
                jsonSerializerSettings: RepositorySettings.CreateDefaultJsonSettings(),
                isTrackingDirty: false,
                backupSettings: null);

            var gitBackuper = new GitBackuper(settings.Directory);
            repository = new SingletonRepository(settings, gitBackuper);
            dummy = new DummySerializable(1);
        }

        [TearDown]
        public static void TearDown()
        {
            DeleteRepositoryDirectory(Directory.FullName);
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            lockFile?.DisposeAndDeleteFile();
        }

        [Test]
        [Explicit("Can't get this to work on AppVeyor")]
        public static async Task SaveCommits()
        {
            // give the repository time to initialize.
            await Task.Delay(500).ConfigureAwait(false);
            using (var git = new LibGit2Sharp.Repository(Directory.FullName))
            {
                Assert.AreEqual(0, git.Commits.Count());
            }

            repository!.Save(dummy);

            using (var git = new LibGit2Sharp.Repository(Directory.FullName))
            {
                var count = 0;
                while (!git.Commits.Any() && count < 10)
                {
                    // give the repository time to update.
                    await Task.Delay(100).ConfigureAwait(false);
                    count++;
                }

                Assert.AreEqual(1, git.Commits.Count());
            }
        }

        [Test]
        public static void ReadOrCreate()
        {
            using (var git = new LibGit2Sharp.Repository(Directory.FullName))
            {
                Assert.AreEqual(0, git.Commits.Count());
            }

            var fileInfo = Directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            var readOrCreate = repository!.ReadOrCreate(fileInfo, () => dummy);
            Assert.AreSame(readOrCreate, dummy);
            using (var git = new LibGit2Sharp.Repository(Directory.FullName))
            {
                Assert.AreEqual(1, git.Commits.Count());
            }
        }

        [Test]
        public static void Restore()
        {
            var file = Directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            Assert.AreEqual(false, repository!.Backuper.CanRestore(file));
            repository!.Save(file, dummy);
            var json = File.ReadAllText(file.FullName);
            Assert.AreEqual("{\r\n  \"Value\": 1\r\n}", json);
            Assert.AreEqual(false, repository!.Backuper.CanRestore(file));
            dummy!.Value++;
            JsonFile.Save(file, dummy);
            json = File.ReadAllText(file.FullName);
            Assert.AreEqual("{\"Value\":2}", json);
            Assert.AreEqual(true, repository.Backuper.CanRestore(file), "CanRestore after save");
            Assert.AreEqual(true, repository.Backuper.TryRestore(file), "TryRestore");
            Assert.AreEqual(false, repository.Backuper.CanRestore(file), "CanRestore after restore");
            var restored = JsonFile.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value - 1, restored.Value);
        }

        [Test]
        public static void TouchDirectoryProp()
        {
            // just so it is not flagged as unused member
            Assert.AreEqual(Directory.FullName, ((GitBackuper)repository!.Backuper).Directory);
        }

        /// <summary>
        /// Recursively deletes a directory as well as any subdirectories and files. If the files are read-only, they are flagged as normal and then deleted.
        /// </summary>
        /// <param name="directory">The name of the directory to remove.</param>
        private static void DeleteRepositoryDirectory(string directory)
        {
            foreach (var subDirectory in System.IO.Directory.EnumerateDirectories(directory))
            {
                DeleteRepositoryDirectory(subDirectory);
            }

            foreach (var fileName in System.IO.Directory.EnumerateFiles(directory))
            {
                var fileInfo = new FileInfo(fileName)
                {
                    Attributes = FileAttributes.Normal,
                };
                fileInfo.Delete();
            }

            System.IO.Directory.Delete(directory, recursive: true);
        }
    }
}