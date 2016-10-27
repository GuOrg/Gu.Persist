namespace Gu.Persist.RuntimeXml.Tests
{
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.Core.Tests.Repositories;
    using Gu.Persist.RuntimeXml;

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
            TestHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return TestHelper.Read<T>(file);
        }
    }
}