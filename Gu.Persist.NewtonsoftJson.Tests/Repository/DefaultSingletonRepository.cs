namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.NewtonsoftJson;

    using NUnit.Framework;

    public class DefaultSingletonRepository : JsonRepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, this.RepoSettingFile);
        }

        protected override IRepository Create()
        {
            return new SingletonRepository();
        }
    }
}
