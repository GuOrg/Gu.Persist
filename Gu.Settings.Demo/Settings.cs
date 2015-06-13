namespace Gu.Settings.Demo
{
    using System.Windows;
    using System.Windows.Data;

    public static class Settings
    {
        public static readonly DependencyProperty ContentProxyProperty = DependencyProperty.RegisterAttached(
            "ContentProxy",
            typeof(object),
            typeof(Settings),
            new FrameworkPropertyMetadata(default(Binding), FrameworkPropertyMetadataOptions.Inherits)
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

        public static void SetContentProxy(DependencyObject element, object value)
        {
            element.SetValue(ContentProxyProperty, value);
        }

        public static object GetContentProxy(DependencyObject element)
        {
            return (object)element.GetValue(ContentProxyProperty);
        }
    }
}
