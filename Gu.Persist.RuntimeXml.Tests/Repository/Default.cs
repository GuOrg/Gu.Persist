#pragma warning disable GU0009 // Name the boolean parameter.
namespace Gu.Persist.RuntimeXml.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using NUnit.Framework;

#pragma warning disable CA1716 // Identifiers should not match keywords
    public class Default : XmlRepositoryTests
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        [Test]
        public void SavesSettingsFile()
        {
            AssertFile.Exists(true, this.RepoSettingFile);
        }

        protected override IRepository CreateRepository()
        {
            return new SingletonRepository();
        }
    }
}