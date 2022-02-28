namespace Gu.Persist.Core.Tests
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TypicalSetting
    {
        private List<DummySerializable> dummies = new();

        public string? Name { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable CA1002 // Do not expose generic lists
        public List<DummySerializable> Dummies
#pragma warning restore CA1002 // Do not expose generic lists
#pragma warning restore CA2227 // Collection properties should be read only
        {
            get => this.dummies;
            set => this.dummies = value;
        }

        public double Value1 { get; set; }

        public double Value2 { get; set; }

        public double Value3 { get; set; }

        public double Value4 { get; set; }

        public double Value5 { get; set; }
    }
}
