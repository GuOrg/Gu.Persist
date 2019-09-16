#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.NewtonsoftJson;

    using NUnit.Framework;
    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class DefaultSingletonRepository : JsonRepositoryTests
    {
        [Test]
        public void SavesSettingsFile()
        {
            var file = this.Directory.CreateFileInfoInDirectory(nameof(RepositorySettings) + ".cfg");
            AssertFile.Exists(false, file);
            _ = this.CreateRepository();
            AssertFile.Exists(true, file);
        }

        protected override IRepository CreateRepository()
        {
            return new SingletonRepository();
        }
    }
}
