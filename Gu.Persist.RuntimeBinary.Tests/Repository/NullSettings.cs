namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using NUnit.Framework;

    public class NullSettings : BinaryRepositoryTests
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
    }
}