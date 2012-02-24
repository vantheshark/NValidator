using System.Collections.Generic;

namespace NValidator.Validators
{
    public class IsNullValidator<T> : BaseNegatableValidator<T> where T : class
    {
        public override bool IsValid(T value, ValidationContext validationContext)
        {
            return value == null;
        }

        public override IEnumerable<ValidationResult> GetErrors(T value, ValidationContext validationContext)
        {
            yield return new ValidationResult
            {
                Message = ErrorMessageProvider.GetError("NValidator_Validators_IsNullValidator_GetErrors")
            };
        }

        public override IEnumerable<ValidationResult> GetNegatableErrors(T value, ValidationContext validationContext)
        {
            yield return new ValidationResult
            {
                Message = ErrorMessageProvider.GetError("NValidator_Validators_IsNullValidator_GetNegatableErrors")
            };
        }
    }
}
