namespace Gu.Wpf.Settings
{
    using System;
    using System.ComponentModel;
    using System.Runtime;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(string))]
    public class TypeNameExtension : MarkupExtension
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public TypeNameExtension()
        {
        }

        public TypeNameExtension(Type type)
        {
            Type = type;
        }

        [ConstructorArgument("type")]
        [DefaultValue(null)]
        public Type Type { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Type.FullName;
        }
    }
}