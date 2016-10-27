// ReSharper disable HeuristicUnreachableCode
namespace Gu.Persist.Core.Tests
{
    using NUnit.Framework;

    public class RemindersTests
    {
        [Test]
        public void ReplaceOldBackupIfNewerThan()
        {
            Assert.Inconclusive("Dunno if a good idea");
        }

        [Test]
        public void MinBackupAgeTest()
        {
            Assert.Inconclusive("Should backups made close to eachother in time be merged some way");
        }

        [Test]
        public void ValidateTimeStampFormat()
        {
            Assert.Inconclusive("roundtrip datetime");
            Assert.Inconclusive("valid filename");
        }

        [Test]
        public void RemoveTimestamp()
        {
            Assert.Inconclusive("roundtrip datetime");
            Assert.Inconclusive("valid filename");
        }

        [Test]
        public void Rename()
        {
            Assert.Inconclusive("repository should handle rename");
            Assert.Inconclusive("Cache should update on rename");
            Assert.Inconclusive("DirtyTracker should update on rename");
            Assert.Inconclusive("Backup should rename backups on rename");
        }

        [Test]
        public void MockFilesystem()
        {
            Assert.Inconclusive();
        }
    }
}
