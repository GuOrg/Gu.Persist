namespace Gu.Persist.Git.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.NewtonsoftJson;

    using NUnit.Framework;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class GitBackuperTests
    {
        private readonly DirectoryInfo directory;
        private DummySerializable dummy;
        private SingletonRepository repository;
        private LockedFile lockFile;

        public GitBackuperTests()
        {
            this.directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\" + this.GetType().Name);
        }

        [SetUp]
        public void SetUp()
        {
            if (this.directory.Exists)
            {
                DeleteRepositoryDirectory(this.directory.FullName);
            }

            this.directory.Create();
            var settings = new RepositorySettings(this.directory.FullName, RepositorySettings.CreateDefaultJsonSettings(), false, null);
            var gitBackuper = new GitBackuper(settings.Directory);
            this.repository = new SingletonRepository(settings, gitBackuper);
            this.dummy = new DummySerializable(1);
        }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var lockFileInfo = Directories.TempDirectory.CreateFileInfoInDirectory("test.lock");
            try
            {
                lockFileInfo.Delete();
            }
            catch
            {
                // this could happen if the previous run was stopped in the debugger.
            }

            // using this because AppVeyor uses two workers for running the tests.
            this.lockFile = await LockedFile.CreateAsync(lockFileInfo, TimeSpan.FromSeconds(1))
                                .ConfigureAwait(false);
        }

        [TearDown]
        public void TearDown()
        {
            this.directory.DeleteIfExists(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.lockFile?.DisposeAndDeleteFile();
        }

        [Test]
        [Explicit("Can't get this to work on AppVeyor")]
        public async Task SaveCommits()
        {
            // give the repository time to initialize.
            await Task.Delay(500).ConfigureAwait(false);
            using (var git = new LibGit2Sharp.Repository(this.directory.FullName))
            {
                Assert.AreEqual(0, git.Commits.Count());
            }

            this.repository.Save(this.dummy);

            using (var git = new LibGit2Sharp.Repository(this.directory.FullName))
            {
                int count = 0;
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
        public void ReadOrCreate()
        {
            using (var git = new LibGit2Sharp.Repository(this.directory.FullName))
            {
                Assert.AreEqual(0, git.Commits.Count());
            }

            var fileInfo = this.directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            var readOrCreate = this.repository.ReadOrCreate(fileInfo, () => this.dummy);
            Assert.AreSame(readOrCreate, this.dummy);
            using (var git = new LibGit2Sharp.Repository(this.directory.FullName))
            {
                Assert.AreEqual(1, git.Commits.Count());
            }
        }

        [Test]
        public void Restore()
        {
            var file = this.directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            Assert.AreEqual(false, this.repository.Backuper.CanRestore(file));
            this.repository.Save(file, this.dummy);
            var json = System.IO.File.ReadAllText(file.FullName);
            Assert.AreEqual("{\r\n  \"Value\": 1\r\n}", json);
            Assert.AreEqual(false, this.repository.Backuper.CanRestore(file));
            this.dummy.Value++;
            JsonFile.Save(file, this.dummy);
            json = System.IO.File.ReadAllText(file.FullName);
            Assert.AreEqual("{\"Value\":2}", json);
            Assert.AreEqual(true, this.repository.Backuper.CanRestore(file), "CanRestore after save");
            Assert.AreEqual(true, this.repository.Backuper.TryRestore(file), "TryRestore");
            Assert.AreEqual(false, this.repository.Backuper.CanRestore(file), "CanRestore after restore");
            var restored = JsonFile.Read<DummySerializable>(file);
            Assert.AreEqual(this.dummy.Value - 1, restored.Value);
        }

        [Test]
        public void TouchDirectoryProp()
        {
            // just so it is not flagged as unused member
            Assert.AreEqual(this.directory.FullName, ((GitBackuper)this.repository.Backuper).Directory);
        }

        /// <summary>
        /// Recursively deletes a directory as well as any subdirectories and files. If the files are read-only, they are flagged as normal and then deleted.
        /// </summary>
        /// <param name="directory">The name of the directory to remove.</param>
        private static void DeleteRepositoryDirectory(string directory)
        {
            foreach (var subdirectory in Directory.EnumerateDirectories(directory))
            {
                DeleteRepositoryDirectory(subdirectory);
            }

            foreach (var fileName in Directory.EnumerateFiles(directory))
            {
                var fileInfo = new FileInfo(fileName)
                {
                    Attributes = FileAttributes.Normal
                };
                fileInfo.Delete();
            }

            Directory.Delete(directory, true);
        }
    }
}