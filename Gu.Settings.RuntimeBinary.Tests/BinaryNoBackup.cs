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
            Assert.AreSame(NullBackuper.Default, this.Repository.Backuper);
        }

        protected override IRepository Create()
        {
            var settings = new BinaryRepositorySettings(this.Directory, true, true, null, ".cfg", ".tmp");
            return new BinaryRepository(settings);
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