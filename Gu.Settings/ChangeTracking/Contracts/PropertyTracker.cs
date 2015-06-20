namespace Gu.Settings
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("Item: {Value.ToString()} ParentType:{ParentType.Name} ParentProperty: {ParentProperty.Name}")]
    public abstract class PropertyTracker : ValueTracker, IPropertyTracker
    {
        protected PropertyTracker(Type parentType, PropertyInfo property, object value)
            : base(value)
        {
            Ensure.NotNull(property, "property");
            ParentType = parentType;
            ParentProperty = property;
        }

        public Type ParentType { get; private set; }

        public PropertyInfo ParentProperty { get; private set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}