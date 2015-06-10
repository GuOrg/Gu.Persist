namespace Gu.Settings.Tests.Helpers
{
    using System;

    [Serializable]
    public class DummySerializable
    {
        public DummySerializable()
        {
        }

        public DummySerializable(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}
