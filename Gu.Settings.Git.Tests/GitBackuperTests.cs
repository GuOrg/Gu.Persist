namespace Gu.Settings.Git.Tests
{
    using System.IO;
    using System.Linq;

    using Gu.Settings.Core.Tests;
    using Gu.Settings.NewtonsoftJson;

    using NUnit.Framework;

    public class GitBackuperTests
    {
        private DummySerializable _dummy;
        private JsonRepository Repository;
        private DirectoryInfo Directory;

        public GitBackuperTests()
        {
            Directory = new DirectoryInfo(@"C:\Temp\Gu.Settings\" + GetType().Name);
            //Directories.Default = Directory;
            _dummy = new DummySerializable(1);

        }

        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists)
            {
                Directory.Delete(true);
            }
            Directory.Create();
            var settings = new JsonRepositorySettings(Directory, null);
            var gitBackuper = new GitBackuper(settings.DirectoryPath);
            Repository = new JsonRepository(settings, gitBackuper);
        }

        [Test]
        public void SaveCommits()
        {
            using (var repository = new LibGit2Sharp.Repository(Directory.FullName))
            {
                Assert.AreEqual(0, repository.Commits.Count());
            }

            Repository.Save(_dummy);
            using (var repository = new LibGit2Sharp.Repository(Directory.FullName))
            {
                Assert.AreEqual(1, repository.Commits.Count());
            }
        }

        protected void Save<T>(T item, FileInfo file)
        {
            TestHelper.Save(item, file);
        }

        protected T Read<T>(FileInfo file)
        {
            return TestHelper.Read<T>(file);
        }
    }
}