namespace Gu.Settings.Core
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Tracks a property
    /// </summary>
    public interface IPropertyTracker : IValueTracker
    {
        /// <summary>
        /// Gets the type of the parent.
        /// </summary>
        Type ParentType { get; }

        /// <summary>
        /// Gets the property of the parent.
        /// </summary>
        PropertyInfo ParentProperty { get; }
    }
}