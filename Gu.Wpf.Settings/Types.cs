namespace Gu.Wpf.Settings
{
    using System;
    using Gu.Settings;

    public static class Types
    {
        public static readonly Type MinMaxDouble = typeof(MinMaxSetting<double>);
        public static readonly Type MinMaxInt = typeof(MinMaxSetting<int>);
        public static readonly Type SettingDouble = typeof(Setting<double>);
        public static readonly Type SettingInt = typeof(Setting<int>);
        public static readonly Type SettingString = typeof(Setting<string>);
        public static readonly Type SettingBool = typeof(Setting<bool>);
    }
}
