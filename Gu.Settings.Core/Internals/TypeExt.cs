namespace Gu.Settings.Core.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class TypeExt
    {
        internal static bool IsEnumerableOfT(this Type type)
        {
            var iEnumerableOfT = type.GetIEnumerableOfT();
            return iEnumerableOfT != null;
        }

        internal static Type GetItemType(this Type type)
        {
            var enumerable = type.GetIEnumerableOfT();
            if (enumerable == null)
            {
                throw new ArgumentException(string.Format("Trying to get typeof(T) when type is not IEnumerable<T>, type is {0}", type.Name), "type");
            }
            return enumerable.GetGenericArguments()
                             .Single();
        }

        private static Type GetIEnumerableOfT(this Type type)
        {
            var enumerable = type.GetInterfaces()
                                 .Where(i => i.IsGenericType)
                                 .SingleOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return enumerable;
        }
    }
}
