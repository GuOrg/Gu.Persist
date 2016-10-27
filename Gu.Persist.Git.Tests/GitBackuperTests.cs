namespace Gu.Persist.Git.Tests
{
    using System.IO;
    using System.Linq;

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
            this.directory = new DirectoryInfo(@"C:\Temp\Gu.Persist\Gu.Persist.Git.Tests\" + this.GetType().Name);
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
        public void SaveCommits()
        {
            var file = this.repository.GetFileInfo<DummySerializable>();
            file.Save(this.dummy);
            using (var git = new LibGit2Sharp.Repository(this.directory.FullName))
            {
                Assert.AreEqual(0, git.Commits.Count());
            }

            this.repository.Save(this.dummy);

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
            var fileInfo = this.directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            Assert.AreEqual(false, this.repository.Backuper.CanRestore(fileInfo));
            this.repository.Save(this.dummy, fileInfo);
            var json = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("{\r\n  \"Value\": 1\r\n}", json);
            Assert.AreEqual(false, this.repository.Backuper.CanRestore(fileInfo));
            this.dummy.Value++;
            this.Save(this.dummy, fileInfo);
            json = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("{\"Value\":2}", json);
            Assert.AreEqual(true, this.repository.Backuper.CanRestore(fileInfo), "CanRestore after save");
            Assert.AreEqual(true, this.repository.Backuper.TryRestore(fileInfo), "TryRestore");
            Assert.AreEqual(false, this.repository.Backuper.CanRestore(fileInfo), "CanRestore after restore");
            var restored = this.Read<DummySerializable>(fileInfo);
            Assert.AreEqual(this.dummy.Value - 1, restored.Value);
        }

        [Test]
        public void TouchDirectoryProp()
        {
            // just so it is not flagged as unused member
            Assert.AreEqual(this.directory.Name, ((GitBackuper)this.repository.Backuper).Directory.CreateDirectoryInfo().Name);
        }

        protected void Save<T>(T item, FileInfo file)
        {
            TestHelper.Save(item, file);
        }

        protected T Read<T>(FileInfo file)
        {
            return TestHelper.Read<T>(file);
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