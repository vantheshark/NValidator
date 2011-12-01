using System.Collections.Generic;
using System.Linq;

namespace NValidator.Validators
{
    public class DoNothingValidator : BaseValidator
    {
        public override IEnumerable<ValidationResult> GetValidationResult(object value, ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
