namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    public class BinaryNoBackup : BinaryRepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, this.Repository.Backuper);
        }

        protected override IRepository Create()
        {
            var settings = new BinaryRepositorySettings(this.Directory, true, true, null, ".cfg", ".tmp");
            return new BinaryRepository(settings);
        }
    }
}