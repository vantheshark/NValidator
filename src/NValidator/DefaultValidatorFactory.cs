using System;
using System.Collections.Generic;
using System.Linq;

namespace NValidator
{
    /// <summary>
    /// DefaultValidatorFactory is the default implementation of IValidatorFactory using a in-memory list to remember all registered validators.
    /// </summary>
    internal class DefaultValidatorFactory : IValidatorFactory
    {
        private static readonly List<IValidator> ValidatorCache = new List<IValidator>();
        private static readonly List<Type> TypeCache = new List<Type>();

        private static readonly List<IValidator> DefaultValidatorCache = new List<IValidator>();
        private static readonly List<Type> DefaultTypeCache = new List<Type>();

        private static readonly object _door = new object();

        internal DefaultValidatorFactory()
        {
            ValidatorCache.Clear();
            TypeCache.Clear();
            DefaultValidatorCache.Clear();
            DefaultTypeCache.Clear();
        }

        private static List<IValidator> GetCache(bool isDefault)
        {
            return isDefault ? DefaultValidatorCache : ValidatorCache;
        }

        private static List<Type> GetTypeCache(bool isDefault)
        {
            return isDefault ? DefaultTypeCache : TypeCache;
        }

        public void Register<T>(bool isDefault = false) where T : class, IValidator
        {
            lock (_door)
            {
                var typeCache = GetTypeCache(isDefault);
                if (!typeCache.Any(x => x == typeof(T)))
                {
                    typeCache.Add(typeof(T));
                }
            }
        }

        public void Register<T>(T validator, bool isDefault = false) where T : class, IValidator
        {
            lock (_door)
            {
                var cache = GetCache(isDefault);
                var existValidator = cache.FirstOrDefault(x => x.GetType() == validator.GetType());
                if (existValidator == null)
                {
                    cache.Add(validator);
                }
                else
                {
                    cache.Remove(existValidator);
                    cache.Add(validator);
                }
            }
        }

        public T Resolve<T>() where T : class, IValidator
        {
            var validator = GetValidator(typeof(T), true) as T ?? GetValidator(typeof(T), false) as T;
            if (validator == null)
            {
                if (typeof (T).IsGenericType)
                {
                    throw new Exception(string.Format("Can not resolve validator for {0}.",
                                                      typeof (T).GetGenericArguments()[0].Name));
                }
                throw new Exception(string.Format("Can not resolve validator {0}.", typeof (T).Name));
            }
            return validator;
        }

        public bool IsDefaultValidator<T>() where T : class, IValidator
        {
            return IsDefaultValidator(typeof (T));
        }

        public bool IsDefaultValidator(Type validatorType)
        {
            if (validatorType.IsInterface || validatorType.IsAbstract)
            {
                throw new ArgumentException("Validator type must be a class", "validatorType");
            }

            var validatorCache = GetCache(true);
            var existMatchedInstance = validatorCache.Any(x => x.GetType().Is(validatorType));

            var typeCache = GetTypeCache(true);
            var existingMatchedType = typeCache.Any(x => x.Is(validatorType));

            return existMatchedInstance || existingMatchedType;
        }

        public IValidator GetValidatorFor(Type objectType)
        {
            var validator = typeof(IValidator<>);
            Type generic = validator.MakeGenericType(objectType);
            return GetValidator(generic, true) as IValidator;
        }

        public IValidator<T> GetValidatorFor<T>() where T : class
        {
            return GetValidatorFor(typeof(T)) as IValidator<T>;
        }

        private static object GetValidator(Type validatorType, bool fromDefaultCache)
        {
            var instanceCache = GetCache(fromDefaultCache);

            var objectInCache = instanceCache.FirstOrDefault(x => x.GetType().Is(validatorType));

            if (objectInCache == null)
            {
                var typeCache = GetTypeCache(fromDefaultCache);
                var type = typeCache.FirstOrDefault(x => x == validatorType || x.Is(validatorType));
                if (type != null)
                {
                    var validator = (IValidator)Activator.CreateInstance(type);
                    return validator;
                }
            }
            return objectInCache;
        }
    }
}
