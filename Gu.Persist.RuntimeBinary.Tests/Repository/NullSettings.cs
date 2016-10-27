namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.Core.Tests.Repositories;
    using NUnit.Framework;

    public class NullSettings : RepositoryTests
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
            BinaryFile.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return BinaryFile.Read<T>(file);
        }
    }
}