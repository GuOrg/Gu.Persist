namespace Gu.Settings.Tests
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
            Assert.Fail("roundtrip datetime");
            Assert.Fail("valid filename");
        }

        [Test]
        public void RemoveTimestamp()
        {
            Assert.Fail("roundtrip datetime");
            Assert.Fail("valid filename");
        }

        [Test]
        public void Rename()
        {
            Assert.Fail("repository should handle rename");
            Assert.Fail("Cache should update on rename");
            Assert.Fail("DirtyTracker should update on rename");
            Assert.Fail("Backup should rename backups on rename");
        }

        [Test]
        public void MockFilesystem()
        {
            Assert.Fail();
        }
    }
}
