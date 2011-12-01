
using System.Collections.Generic;

namespace NValidator
{
    public interface IValidator
    {
        bool IsValid(object value);
        bool IsValid(object value, ValidationContext validationContext);

        IEnumerable<ValidationResult> GetValidationResult(object value);
        IEnumerable<ValidationResult> GetValidationResult(object value, ValidationContext validationContext);
    }

    public interface IValidator<in T> : IValidator
    {
        bool IsValid(T value);
        bool IsValid(T value, ValidationContext validationContext);

        IEnumerable<ValidationResult> GetValidationResult(T value);
        IEnumerable<ValidationResult> GetValidationResult(T value, ValidationContext validationContext);
    }
}
