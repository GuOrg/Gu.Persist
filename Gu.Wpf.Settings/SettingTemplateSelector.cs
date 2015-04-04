namespace Gu.Wpf.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    [ContentProperty("Templates")]
    public class SettingTemplateSelector : DataTemplateSelector
    {
        public SettingTemplateSelector()
        {
            Templates = new List<DataTemplate>();
        }

        public List<DataTemplate> Templates { get; set; }
        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null || container == null)
            {
                return base.SelectTemplate(item, container);
            }
            var type = item.GetType();
            return Templates.SingleOrDefault(x => x.DataType == type);
            throw new ArgumentOutOfRangeException("item");
        }
    }
}
