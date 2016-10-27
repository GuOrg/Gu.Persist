namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using Gu.Persist.Core.Tests.Repositories;
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
            BinaryFile.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return BinaryFile.Read<T>(file);
        }
    }
}