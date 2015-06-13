namespace Gu.Settings.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Gu.Settings.Tests.Helpers;

    using NUnit.Framework;

    public class ComparerTests
    {
        private TypicalSetting _setting1;
        private TypicalSetting _setting2;

        [SetUp]
        public void SetUp()
        {
            _setting1 = new TypicalSetting
                            {
                                Name = "Johan Larsson",
                                Dummies = Enumerable.Range(0, 1).Select(x => new DummySerializable(x)).ToList(),
                                Value1 = 1.2,
                                Value2 = 2,
                                Value3 = 3,
                                Value4 = 4,
                                Value5 = 5
                            };
            _setting2 = BinaryHelper.Clone(_setting1);
        }

        [TestCaseSource(typeof(ComparerSource))]
        public void WhenEqual(IEqualityComparer<TypicalSetting> comparer)
        {
            Assert.IsTrue(comparer.Equals(_setting1, _setting2));
            Assert.AreEqual(comparer.GetHashCode(_setting1), comparer.GetHashCode(_setting2));
        }

        [TestCaseSource(typeof(ComparerSource))]
        public void WhenNot(IEqualityComparer<TypicalSetting> comparer)
        {
            _setting2.Value1++;
            Assert.AreNotEqual(_setting1.Value1, _setting2.Value1);
            Assert.IsFalse(comparer.Equals(_setting1, _setting2));

            Assert.AreNotEqual(comparer.GetHashCode(_setting1), comparer.GetHashCode(_setting2));
        }

        public class ComparerSource : List<IEqualityComparer<TypicalSetting>>
        {
            public ComparerSource()
            {
                Add(XmlEqualsComparer<TypicalSetting>.Default);
                Add(BinaryEqualsComparer<TypicalSetting>.Default);
            }
        }
    }
}
