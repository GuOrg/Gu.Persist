namespace Gu.Persist.SystemXml.Tests
{
    using System.Linq;
    using Gu.Persist.Core.Tests;

    using NUnit.Framework;

    public class XmlEqualsComparerTests
    {
        private TypicalSetting setting1;
        private TypicalSetting setting2;

        [SetUp]
        public void SetUp()
        {
            this.setting1 = new TypicalSetting
                            {
                                Name = "Johan Larsson",
                                Dummies = Enumerable.Range(0, 1)
                                                    .Select(x => new DummySerializable(x))
                                                    .ToList(),
                                Value1 = 1.2,
                                Value2 = 2,
                                Value3 = 3,
                                Value4 = 4,
                                Value5 = 5,
                            };

            this.setting2 = new TypicalSetting
                            {
                                Name = "Johan Larsson",
                                Dummies = Enumerable.Range(0, 1)
                                                    .Select(x => new DummySerializable(x))
                                                    .ToList(),
                                Value1 = 1.2,
                                Value2 = 2,
                                Value3 = 3,
                                Value4 = 4,
                                Value5 = 5,
                            };
        }

        [Test]
        public void WhenEqual()
        {
            var comparer = XmlEqualsComparer<TypicalSetting>.Default;
            Assert.IsTrue(comparer.Equals(this.setting1, this.setting2));
            Assert.AreEqual(comparer.GetHashCode(this.setting1), comparer.GetHashCode(this.setting2));
        }

        [Test]
        public void WhenNotEqual()
        {
            var comparer = XmlEqualsComparer<TypicalSetting>.Default;
            this.setting2.Value1++;
            Assert.AreNotEqual(this.setting1.Value1, this.setting2.Value1);
            Assert.IsFalse(comparer.Equals(this.setting1, this.setting2));

            Assert.AreNotEqual(comparer.GetHashCode(this.setting1), comparer.GetHashCode(this.setting2));
        }
    }
}
