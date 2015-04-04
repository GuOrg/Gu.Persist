namespace Gu.Wpf.Settings
{
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Settings;

    public class SettingTemplateSelector : DataTemplateSelector
    {
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
