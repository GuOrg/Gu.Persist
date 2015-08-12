namespace Gu.Settings.RuntimeBinary.Tests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Backup;
    using Gu.Settings.Core.Tests.Repositories;

    using NUnit.Framework;

    public class BinaryNoBackup : RepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, Repository.Backuper);
        }

        protected override RepositorySettings Settings
        {
            get { return new RepositorySettings(Directory, true, true, BackupSettings, ".cfg", ".tmp"); }
        }

        protected override BackupSettings BackupSettings
        {
            get { return null; }
        }

        protected override IRepository Create()
        {
            return new BinaryRepository(Settings);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            BinaryHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return BinaryHelper.Read<T>(file);
        }
    }
}