namespace Gu.Settings
{
    using System;
    using System.Reflection;

    public interface IPropertyTracker : IValueTracker
    {
        Type ParentType { get; }

        PropertyInfo ParentProperty { get; }
    }
}