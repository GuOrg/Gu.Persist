namespace Gu.Settings.Core
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
            Ensure.NotNull(property, nameof(property));
            ParentType = parentType;
            ParentProperty = property;
        }

        /// <inheritdoc/>
        public Type ParentType { get; private set; }

        /// <inheritdoc/>
        public PropertyInfo ParentProperty { get; private set; }
    }
}