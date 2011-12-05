using System;

namespace NValidator
{
    internal static class TypeExtensions
    {
        public static bool Is<T>(this Type type)
        {
            return Is(type, typeof (T));
        }

        public static bool Is(this Type type, Type parentType)
        {
            return parentType.IsAssignableFrom(type) || IsSubclassOfRawGeneric(parentType, type);
        }
        //http://stackoverflow.com/questions/457676/c-sharp-reflection-check-if-a-class-is-derived-from-a-generic-class
        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        internal static bool IsMoreConcreteGenericParam(this Type validatorA, Type validatorB)
        {
            if (IsSubclassOfRawGeneric(validatorA, typeof(IValidator<>)) &&
                IsSubclassOfRawGeneric(validatorB, typeof(IValidator<>)) != true)
            {
                throw new Exception("One of the validator does not implement IValidator<>");
            }

            Type genericParamTypeA = GetGenericParamOfValidator(validatorA);
            Type genericParamTypeB = GetGenericParamOfValidator(validatorB);

            return genericParamTypeA != null &&
                   genericParamTypeB != null &&
                   genericParamTypeB.IsAssignableFrom(genericParamTypeA);
        }

        private static Type GetGenericParamOfValidator(Type validatorType)
        {
            var genericTypeOfB = validatorType;
            while (genericTypeOfB != null && !genericTypeOfB.IsGenericType)
            {
                genericTypeOfB = genericTypeOfB.BaseType;
            }
            return genericTypeOfB != null ? genericTypeOfB.GetGenericArguments()[0] : null;
        }
    }
}
