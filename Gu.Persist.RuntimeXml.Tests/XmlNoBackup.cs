namespace Gu.Persist.RuntimeXml.Tests
{
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;
    using Gu.Persist.Core.Tests.Repositories;

    using NUnit.Framework;

    public class XmlNoBackup : RepositoryTests
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
            XmlTestHelper.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return XmlTestHelper.Read<T>(file);
        }
    }
}