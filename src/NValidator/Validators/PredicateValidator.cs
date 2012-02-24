using System;
using System.Collections.Generic;

namespace NValidator.Validators
{
    public class PredicateValidator<T> : BaseValidator<T>
    {
        private readonly Func<T, bool> _predicate;
        protected PredicateValidator()
        {
        }

        public PredicateValidator(Func<T, bool> predicate)
        {
            _predicate = predicate;
        }

        public override bool IsValid(T value, ValidationContext validationContext)
        {
            try
            {
                return _predicate(value);
            }
            catch
            {
                return false;
            }
        }

        public sealed override IEnumerable<ValidationResult> GetValidationResult(T value, ValidationContext validationContext)
        {
            if (!IsValid(value, validationContext))
            {
                yield return new ValidationResult
                {
                    Message = ErrorMessageProvider.GetError("NValidator_Validators_PredicateValidator_GetValidationResult")
                };
            }
        }
    }

    public class PredicateValidator<T, TProperty> : PredicateValidator<TProperty> where T : class 
    {
        private readonly Func<T, TProperty,  bool> _predicate;

        public PredicateValidator(Func<T, TProperty, bool> predicate)
        {
            _predicate = predicate;
        }

        public sealed override bool IsValid(TProperty value, ValidationContext validationContext)
        {
            return _predicate(validationContext.ContainerInstance as T, value);
        }
    }
}
