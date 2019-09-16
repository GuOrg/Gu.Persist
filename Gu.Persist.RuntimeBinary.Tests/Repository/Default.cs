#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using NUnit.Framework;

#pragma warning disable CA1716 // Identifiers should not match keywords
    public class Default : BinaryRepositoryTests
#pragma warning restore CA1716 // Identifiers should not match keywords
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