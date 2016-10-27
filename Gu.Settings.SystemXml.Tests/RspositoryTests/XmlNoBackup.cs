namespace Gu.Settings.SystemXml.Tests.RspositoryTests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Backup;
    using Gu.Settings.Core.Tests.Repositories;
    using Gu.Settings.SystemXml.Tests.Helpers;

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
            TestHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return TestHelper.Read<T>(file);
        }
    }
}