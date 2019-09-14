namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using NUnit.Framework;

    public class NoBackup : BinaryRepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, this.Repository.Backuper);
        }

        protected override IRepository Create()
        {
            var settings = new RepositorySettings(
                directory: this.Directory.FullName,
                isTrackingDirty: true,
                backupSettings: null);
            return new SingletonRepository(settings);
        }
    }
}