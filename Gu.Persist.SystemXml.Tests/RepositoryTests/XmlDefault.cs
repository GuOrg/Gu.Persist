namespace Gu.Persist.SystemXml.Tests.RepositoryTests
{
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.Core.Tests.Repositories;
    using Gu.Persist.SystemXml;

    using NUnit.Framework;

    public class XmlDefault : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, this.RepoSettingFile);
        }

        protected override IRepository Create()
        {
            return new XmlRepository();
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