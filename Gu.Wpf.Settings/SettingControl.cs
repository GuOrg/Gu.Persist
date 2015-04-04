namespace Gu.Wpf.Settings
{
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Settings;

    public class SettingControl : HeaderedContentControl
    {
        public static readonly DependencyProperty SettingProperty = DependencyProperty.Register(
            "Setting", 
            typeof(ISetting),
            typeof(SettingControl), 
            new PropertyMetadata(default(ISetting), OnSettingChanged));

        static SettingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SettingControl),
                new FrameworkPropertyMetadata(typeof(SettingControl)));

            ContentProperty.OverrideMetadata(
                typeof(SettingControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable));
        }

        public ISetting Setting
        {
            get { return (ISetting)GetValue(SettingProperty); }
            set { SetValue(SettingProperty, value); }
        }

        private static void OnSettingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((SettingControl)o).Content = e.NewValue;
        }
    }
}
