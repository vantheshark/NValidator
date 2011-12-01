
using System;
using System.Collections.Generic;
using System.Linq;

namespace NValidator
{
    public interface INegatableValidator<in T>
    {
        bool IsValid(T value, ValidationContext validationContext);
        IEnumerable<ValidationResult> GetErrors(T value, ValidationContext validationContext);
        IEnumerable<ValidationResult> GetNegatableErrors(T value, ValidationContext validationContext);
    }

    public abstract class BaseNegatableValidator<T> : BaseValidator<T>, INegatableValidator<T>
    {
        public abstract IEnumerable<ValidationResult> GetErrors(T value, ValidationContext validationContext);

        public abstract IEnumerable<ValidationResult> GetNegatableErrors(T value, ValidationContext validationContext);

        public override bool IsValid(T value, ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        public sealed override IEnumerable<ValidationResult> GetValidationResult(T value, ValidationContext validationContext)
        {
            if (!IsValid(value, validationContext))
            {
                return GetErrors(value, validationContext);
            }
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
