namespace Gu.Wpf.Settings.Demo
{
    using Gu.Settings;

    public class SampleSetting : SettingCollection
    {
        public SampleSetting()
        {
            DoubleMinMax = new MinMaxSetting<double> { Min = -5, Max = 5 };
            IntMinMax = new MinMaxSetting<int> { Min = -5, Max = 5 };
            SettingBool = new Setting<bool>(false);
            SettingInt = new Setting<int>(false);
            SettingDouble = new Setting<double>(false);
        }

        public MinMaxSetting<double> DoubleMinMax { get; set; }
        public MinMaxSetting<int> IntMinMax { get; set; }
        public Setting<bool> SettingBool { get; set; }
        public Setting<int> SettingInt { get; set; }
        public Setting<double> SettingDouble { get; set; }
    }
}
