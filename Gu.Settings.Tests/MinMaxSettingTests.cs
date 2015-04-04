namespace Gu.Settings.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using NUnit.Framework;

    public class MinMaxSettingTests
    {
        [TestCase(1, 2, false)]
        [TestCase(2, 1, true)]
        [TestCase(null, 1, true)]
        public void Max(int? max, int value, bool expected)
        {
            var setting = new MinMaxSetting<int>
                              {
                                  Setting = new Setting<int>(false)
                              };
            setting.Max = max;
            setting.Setting.EditValue = value;
            Assert.AreEqual(expected, setting.IsValid);
        }

        [TestCase(1, 2, true)]
        [TestCase(2, 1, false)]
        [TestCase(null, 1, true)]
        public void Min(int? min, int value, bool expected)
        {
            var setting = new MinMaxSetting<int>
            {
                Setting = new Setting<int>(false)
            };
            setting.Min = min;
            setting.Setting.EditValue = value;
            Assert.AreEqual(expected, setting.IsValid);
        }
    }
}
