namespace Gu.Persist.Git.Tests
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.NewtonsoftJson;

    using NUnit.Framework;

    public class GitBackuperTests
    {
        private readonly DirectoryInfo directory;
        private DummySerializable dummy;
        private JsonRepository repository;

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
            var settings = new JsonRepositorySettings(this.directory, null);
            var gitBackuper = new GitBackuper(settings.DirectoryPath);
            this.repository = new JsonRepository(settings, gitBackuper);
            this.dummy = new DummySerializable(1);
        }

        [Test]
        public async Task SaveCommits()
        {
            ////var file = this.repository.GetFileInfo<DummySerializable>();
            ////file.Save(this.dummy);
            using (var git = new LibGit2Sharp.Repository(this.directory.FullName))
            {
                Assert.AreEqual(0, git.Commits.Count());
            }

            this.repository.Save(this.dummy);
            await Task.Delay(100).ConfigureAwait(false);
            using (var git = new LibGit2Sharp.Repository(this.directory.FullName))
            {
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
            this.repository.Save(this.dummy, file);
            var json = File.ReadAllText(file.FullName);
            Assert.AreEqual("{\r\n  \"Value\": 1\r\n}", json);
            Assert.AreEqual(false, this.repository.Backuper.CanRestore(file));
            this.dummy.Value++;
            JsonFile.Save(file, this.dummy);
            json = File.ReadAllText(file.FullName);
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
            Assert.AreEqual(this.directory.Name, ((GitBackuper)this.repository.Backuper).Directory.CreateDirectoryInfo().Name);
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