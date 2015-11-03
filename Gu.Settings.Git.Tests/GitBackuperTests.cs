namespace Gu.Settings.Git.Tests
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Core;
    using Gu.Settings.Core.Tests;
    using Gu.Settings.NewtonsoftJson;

    using NUnit.Framework;

    public class GitBackuperTests
    {
        private DummySerializable _dummy;
        private JsonRepository _repository;
        private readonly DirectoryInfo _directory;

        public GitBackuperTests()
        {
            _directory = new DirectoryInfo(@"C:\Temp\Gu.Settings\" + GetType().Name);
        }

        [SetUp]
        public void SetUp()
        {
            if (_directory.Exists)
            {
                DeleteRepositoryDirectory(_directory.FullName);
            }
            _directory.Create();
            var settings = new JsonRepositorySettings(_directory, null);
            var gitBackuper = new GitBackuper(settings.DirectoryPath);
            _repository = new JsonRepository(settings, gitBackuper);
            _dummy = new DummySerializable(1);
        }

        [Test]
        public void SaveCommits()
        {
            using (var repository = new LibGit2Sharp.Repository(_directory.FullName))
            {
                Assert.AreEqual(0, repository.Commits.Count());
            }

            _repository.Save(_dummy);

            using (var repository = new LibGit2Sharp.Repository(_directory.FullName))
            {
                Assert.AreEqual(1, repository.Commits.Count());
            }
        }

        [Test]
        public void ReadOrCreate()
        {
            using (var repository = new LibGit2Sharp.Repository(_directory.FullName))
            {
                Assert.AreEqual(0, repository.Commits.Count());
            }
            var fileInfo = _directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            var readOrCreate = _repository.ReadOrCreate(fileInfo, () => _dummy);
            Assert.AreSame(readOrCreate, _dummy);
            using (var repository = new LibGit2Sharp.Repository(_directory.FullName))
            {
                Assert.AreEqual(1, repository.Commits.Count());
            }
        }

        [Test]
        public void Restore()
        {
            var fileInfo = _directory.CreateFileInfoInDirectory(nameof(DummySerializable) + ".cfg");
            Assert.AreEqual(false, _repository.Backuper.CanRestore(fileInfo));
            _repository.Save(_dummy, fileInfo);
            var json = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("{\r\n  \"Value\": 1\r\n}", json);
            Assert.AreEqual(false, _repository.Backuper.CanRestore(fileInfo));
            _dummy.Value++;
            Save(_dummy, fileInfo);
            json = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("{\"Value\":2}", json);
            Assert.AreEqual(true, _repository.Backuper.CanRestore(fileInfo), "CanRestore after save");
            Assert.AreEqual(true, _repository.Backuper.TryRestore(fileInfo), "TryRestore");
            Assert.AreEqual(false, _repository.Backuper.CanRestore(fileInfo), "CanRestore after restore");
            var restored = Read<DummySerializable>(fileInfo);
            Assert.AreEqual(_dummy.Value - 1, restored.Value);
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
        public static void DeleteRepositoryDirectory(string directory)
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
            Directory.Delete(directory);
        }
    }
}