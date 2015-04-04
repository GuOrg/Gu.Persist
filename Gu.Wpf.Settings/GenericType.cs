namespace Gu.Wpf.Settings
{
    using System;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(Type))]
    public class GenericType : MarkupExtension
    {
        public GenericType()
        {
        }

        public GenericType(Type baseType, Type innerType)
        {
            BaseType = baseType;
            InnerType = innerType;
        }

        public Type BaseType { get; set; }

        public Type InnerType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Type result = BaseType.MakeGenericType(InnerType);
            return result;
        }
    }
}