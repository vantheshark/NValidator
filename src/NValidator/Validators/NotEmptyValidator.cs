using System.Collections;
using System.Collections.Generic;

namespace NValidator.Validators
{
    public class NotEmptyValidator<T> : BaseValidator<T> where T : IEnumerable
    {
        public override IEnumerable<ValidationResult> GetValidationResult(T value, ValidationContext validationContext)
        {
            var enumerable = value as IEnumerable;
            if (enumerable != null && enumerable.GetEnumerator().MoveNext())
            {
                yield break;
            }
            yield return new ValidationResult
            {
                Message = ErrorMessageProvider.GetError("NValidator_Validators_NotEmptyValidator_GetValidationResult")
            };
        }
    }
}
