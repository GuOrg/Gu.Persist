namespace Gu.Persist.RuntimeBinary.Tests
{
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.Core.Tests.Repositories;

    using NUnit.Framework;

    public class BinaryRepositoryNullSettings : RepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, this.RepoSettingFile);
        }

        protected override IRepository Create()
        {
            return new BinaryRepository();
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