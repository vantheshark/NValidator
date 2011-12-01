using System;

namespace NValidator
{
    internal static class TypeExtensions
    {
        public static bool Is<T>(this Type type)
        {
            return typeof (T).IsAssignableFrom(type);
        }

        public static bool Is(this Type type, Type parentType)
        {
            return parentType.IsAssignableFrom(type);
        }
    }
}
