namespace Gu.Persist.SystemXml.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using Gu.Persist.Core.Tests.Repositories;
    using NUnit.Framework;

    public class NoBackup : RepositoryTests
    {
        [Test]
        public void BackuperIsNone()
        {
            Assert.AreSame(NullBackuper.Default, this.Repository.Backuper);
        }

        protected override IRepository Create()
        {
            var settings = new XmlRepositorySettings(this.Directory, null);
            return new XmlRepository(settings);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            XmlFile.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return XmlFile.Read<T>(file);
        }
    }
}