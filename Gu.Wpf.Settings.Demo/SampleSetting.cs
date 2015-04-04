namespace Gu.Wpf.Settings.Demo
{
    using Gu.Settings;

    public class SampleSetting : SettingCollection
    {
        public SampleSetting()
        {
            DoubleMinMax = new MinMaxSetting<double> {Min = -5, Max = 5};
        }

        public MinMaxSetting<double> DoubleMinMax { get; set; }
    }
}
