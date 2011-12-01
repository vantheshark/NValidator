using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NValidator.Validators
{
    public class RegularExpressionValidator : BaseValidator<string>
    {
        private readonly string _pattern;
        private readonly string _messageFormat;

        public RegularExpressionValidator(string pattern, string messageFormat = "@PropertyName is not in the correct format.")
        {
            _pattern = pattern;
            _messageFormat = messageFormat;
        }

        public override IEnumerable<ValidationResult> GetValidationResult(string value, ValidationContext validationContext)
        {
            if (value != null && Regex.IsMatch(value, _pattern))
            {
                yield break;
            }
            yield return new ValidationResult
            {
                Message = _messageFormat
            };
        }
    }
}
