namespace Gu.Persist.Core.Tests
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TypicalSetting
    {
        private List<DummySerializable> dummies = new List<DummySerializable>();

        public string Name { get; set; }

        public List<DummySerializable> Dummies
        {
            get { return this.dummies; }
            set { this.dummies = value; }
        }

        public double Value1 { get; set; }

        public double Value2 { get; set; }

        public double Value3 { get; set; }

        public double Value4 { get; set; }

        public double Value5 { get; set; }
    }
}
