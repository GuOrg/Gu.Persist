namespace Gu.Wpf.Settings
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Settings;

    public class SettingTemplateSelector : DataTemplateSelector
    {
        public static Type DoubleMinMaxType = typeof (MinMaxSetting<double>);
       
        public DataTemplate DoubleMinMaxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MinMaxSetting<double>)
            {
                return DoubleMinMaxTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
