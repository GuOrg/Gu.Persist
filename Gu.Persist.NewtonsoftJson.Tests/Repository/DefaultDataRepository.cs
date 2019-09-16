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
            var file = this.Directory.CreateFileInfoInDirectory(nameof(DataRepositorySettings) + ".cfg");
            AssertFile.Exists(false, file);
            _ = this.CreateRepository();
            AssertFile.Exists(true, file);
        }

        protected override IRepository CreateRepository()
        {
            return new DataRepository();
        }
    }
}