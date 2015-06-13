namespace Gu.Settings.Tests
{
    using Helpers;
    using NUnit.Framework;

    public class SimpleSample
    {
        [Test]
        public void XmlSample()
        {
            var repository = new XmlRepository();
            var setting = repository.ReadOrCreate(() => new DummySerializable());
            setting.Value ++;
            Assert.IsTrue(repository.IsDirty(setting));
            repository.Save(setting);
            Assert.IsFalse(repository.IsDirty(setting));
        }
    }
}
