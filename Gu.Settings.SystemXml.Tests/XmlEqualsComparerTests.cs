namespace Gu.Settings.SystemXml.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests;

    using NUnit.Framework;

    public class XmlEqualsComparerTests
    {
        private TypicalSetting _setting1;
        private TypicalSetting _setting2;

        [SetUp]
        public void SetUp()
        {
            _setting1 = new TypicalSetting
                            {
                                Name = "Johan Larsson",
                                Dummies = Enumerable.Range(0, 1)
                                                    .Select(x => new DummySerializable(x))
                                                    .ToList(),
                                Value1 = 1.2,
                                Value2 = 2,
                                Value3 = 3,
                                Value4 = 4,
                                Value5 = 5
                            };

            _setting2 = new TypicalSetting
                            {
                                Name = "Johan Larsson",
                                Dummies = Enumerable.Range(0, 1)
                                                    .Select(x => new DummySerializable(x))
                                                    .ToList(),
                                Value1 = 1.2,
                                Value2 = 2,
                                Value3 = 3,
                                Value4 = 4,
                                Value5 = 5
                            };
        }

        [Test]
        public void WhenEqual()
        {
            var comparer = XmlEqualsComparer<TypicalSetting>.Default;
            Assert.IsTrue(comparer.Equals(_setting1, _setting2));
            Assert.AreEqual(comparer.GetHashCode(_setting1), comparer.GetHashCode(_setting2));
        }

        [Test]
        public void WhenNotEqual()
        {
            var comparer = XmlEqualsComparer<TypicalSetting>.Default;
            _setting2.Value1++;
            Assert.AreNotEqual(_setting1.Value1, _setting2.Value1);
            Assert.IsFalse(comparer.Equals(_setting1, _setting2));

            Assert.AreNotEqual(comparer.GetHashCode(_setting1), comparer.GetHashCode(_setting2));
        }
    }
}
