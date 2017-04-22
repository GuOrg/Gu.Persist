#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;

    using NUnit.Framework;

    public class DefaultDataRepository : JsonRepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, this.RepoSettingFile);
        }

        protected override IRepository Create()
        {
            return new DataRepository();
        }
    }
}