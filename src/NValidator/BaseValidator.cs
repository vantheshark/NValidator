using System.Collections.Generic;
using System.Linq;

namespace NValidator
{
    public abstract class BaseValidator : IValidator
    {
        public bool IsValid(object value)
        {
            return IsValid(value, null);
        }

        public virtual bool IsValid(object value, ValidationContext validationContext)
        {
            var result = GetValidationResult(value, validationContext);
            return result == null || result.Count() == 0;
        }

        public IEnumerable<ValidationResult> GetValidationResult(object value)
        {
            return GetValidationResult(value, null);
        }

        public abstract IEnumerable<ValidationResult> GetValidationResult(object value, ValidationContext validationContext);
    }

    public abstract class BaseValidator<T> : BaseValidator, IValidator<T>
    {
        public sealed override IEnumerable<ValidationResult> GetValidationResult(object value, ValidationContext validationContext)
        {
            return GetValidationResult((T) value, validationContext);
        }

        public bool IsValid(T value)
        {
            return IsValid(value, null);
        }

        public virtual bool IsValid(T value, ValidationContext validationContext)
        {
            var result = GetValidationResult(value, validationContext);
            return result == null || result.Count() == 0;
        }

        public IEnumerable<ValidationResult> GetValidationResult(T value)
        {
            return GetValidationResult(value, null);
        }

        public abstract IEnumerable<ValidationResult> GetValidationResult(T value, ValidationContext validationContext);
    }
}
