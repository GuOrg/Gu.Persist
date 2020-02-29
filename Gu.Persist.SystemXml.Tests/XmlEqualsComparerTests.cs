namespace Gu.Persist.SystemXml.Tests
{
    using System.Linq;
    using Gu.Persist.Core.Tests;

    using NUnit.Framework;

    public class XmlEqualsComparerTests
    {
        [Test]
        public void WhenEqual()
        {
            var setting1 = new TypicalSetting
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

            var setting2 = new TypicalSetting
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

            var comparer = XmlEqualsComparer<TypicalSetting>.Default;
            Assert.IsTrue(comparer.Equals(setting1, setting2));
            Assert.AreEqual(comparer.GetHashCode(setting1), comparer.GetHashCode(setting2));
        }

        [Test]
        public void WhenNotEqual()
        {
            var setting1 = new TypicalSetting
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

            var setting2 = new TypicalSetting
            {
                Name = "Johan Larsson",
                Dummies = Enumerable.Range(0, 1)
                                    .Select(x => new DummySerializable(x))
                                    .ToList(),
                Value1 = 2.3,
                Value2 = 2,
                Value3 = 3,
                Value4 = 4,
                Value5 = 5,
            };

            var comparer = XmlEqualsComparer<TypicalSetting>.Default;
            Assert.AreNotEqual(setting1.Value1, setting2.Value1);
            Assert.IsFalse(comparer.Equals(setting1, setting2));

            Assert.AreNotEqual(comparer.GetHashCode(setting1), comparer.GetHashCode(setting2));
        }
    }
}
