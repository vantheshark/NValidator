using System;
using NValidator.Validators;

namespace NValidator
{
    public interface IValidatorFactory
    {
        /// <summary>
        /// Registers the specified validator type.
        /// <para>If a validator is registered as default, it will be resolvable when the client call "GetValidatorFor".</para>
        /// </summary>
        void Register<T>(bool isDefault = false) where T : class, IValidator;

        /// <summary>
        /// Registers the specified validator type.
        /// <para>If a validator is registered as default, it will be resolvable when the client call "GetValidatorFor".</para>
        /// </summary>
        void Register<T>(T validator, bool isDefault = false) where T : class, IValidator;

        /// <summary>
        /// Resolves any registered validator include none-default one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class, IValidator;
        
        /// <summary>
        /// Determines whether the validator type is registered as default validator.
        /// </summary>
        bool IsDefaultValidator<T>() where T : class, IValidator;
        
        /// <summary>
        /// Determines whether the validator type is default validator.
        /// </summary>
        bool IsDefaultValidator(Type validatorType);

        /// <summary>
        /// Gets the "DEFAULT" validator for a type.
        /// <para>Only validator registered as default will be returned.</para>
        /// </summary>
        IValidator GetValidatorFor(Type objectType);

        /// <summary>
        /// Gets the "DEFAULT" validator for a type.
        /// <para>Only validator registered as default will be returned.</para>
        /// </summary>
        IValidator<T> GetValidatorFor<T>() where T : class;
    }

    internal class NullValidatorFactory : IValidatorFactory
    {
        public void Register<T>(bool isDefault) where T : class, IValidator
        {
        }

        public void Register<T>(T validator, bool isDefault) where T : class, IValidator
        {
        }

        public T Resolve<T>() where T : class, IValidator
        {
            return default(T);
        }

        public bool IsDefaultValidator<T>() where T : class, IValidator
        {
            return false;
        }

        public bool IsDefaultValidator(Type validatorType)
        {
            return false;
        }

        public IValidator GetValidatorFor(Type objectType)
        {
            return null;
        }

        public IValidator<T> GetValidatorFor<T>() where T : class
        {
            return null;
        }
    }

    public static class ValidatorFactory
    {
        static ValidatorFactory()
        {
            Current = new DefaultValidatorFactory();
            NullValidator = new DoNothingValidator();
        }

        public static IValidator NullValidator { get; private set; }

        public static IValidatorFactory Current { get; set; }
    }
}
