namespace Gu.Settings.SystemXml.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Gu.Settings.SystemXml;
    using Gu.Settings.Tests;

    using NUnit.Framework;

    [Explicit("Longrunning benchmarks")]
    public class BenchmarkTests
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
        public void XmlEquals(ComparerData data)
        {
            var comparer = XmlEqualsComparer<TypicalSetting>.Default;
            var warmup = comparer.Equals(_setting1, _setting2);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < data.Times; i++)
            {
                var result = comparer.Equals(_setting1, _setting2);
            }
            sw.Stop();
            Console.WriteLine(
                "{0}.Equals(_setting1, _setting2) {1} times took: {2} ms total ({3} ms each)",
                data.ComparerName,
                data.Times,
                sw.ElapsedMilliseconds,
                sw.Elapsed.TotalMilliseconds / data.Times);
        }

        [TestCaseSource(typeof(ComparerSource))]
        public void GetHashCode(ComparerData data)
        {
            var comparer = XmlEqualsComparer<TypicalSetting>.Default;
            var warmup = comparer.GetHashCode(_setting1);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < data.Times; i++)
            {
                var result = comparer.GetHashCode(_setting1);
            }
            sw.Stop();
            Console.WriteLine(
                "{0}.GetHashCode(_setting1) {1} times took: {2} ms total ({3} ms each)",
                data.ComparerName,
                data.Times,
                sw.ElapsedMilliseconds,
                sw.Elapsed.TotalMilliseconds / data.Times);
        }

        public class ComparerSource : List<ComparerData>
        {
            public ComparerSource()
            {
                Add(new ComparerData(XmlEqualsComparer<TypicalSetting>.Default, 1000));
                Add(new ComparerData(BinaryEqualsComparer<TypicalSetting>.Default, 1000));
            }
        }

        public class ComparerData
        {
            public readonly IEqualityComparer<TypicalSetting> Comparer;
            public readonly int Times;

            public ComparerData(IEqualityComparer<TypicalSetting> comparer, int times)
            {
                Comparer = comparer;
                Times = times;
            }

            public string ComparerName
            {
                get
                {
                    return string.Format(
                        "{0}<{1}>",
                        Comparer.GetType()
                                .Name.Replace("`1", ""),
                        typeof(TypicalSetting).Name);
                }
            }

            public override string ToString()
            {
                return string.Format("{0} {1} times", ComparerName, Times);
            }
        }
    }
}
