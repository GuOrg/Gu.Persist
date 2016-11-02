namespace Gu.Persist.Yaml.Tests.Repository
{
    using Core;
    using NUnit.Framework;

    public class Default : YamlRepositoryTests
    {
        [Test]
        public void CreateSecond()
        {
            Assert.DoesNotThrow(() => this.Create());
        }

        protected override IRepository Create()
        {
            return new SingletonRepository();
        }
    }
}