using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NValidator.Validators
{
    public class RegularExpressionValidator : BaseValidator<string>
    {
        private readonly string _pattern;
        private readonly bool _allowNull;
        private readonly string _messageFormat;

        public RegularExpressionValidator(string pattern, bool allowNull = true, string messageFormat = "@PropertyName is not in the correct format.")
        {
            _pattern = pattern;
            _allowNull = allowNull;
            _messageFormat = messageFormat;
        }

        public override IEnumerable<ValidationResult> GetValidationResult(string value, ValidationContext validationContext)
        {
            if (_allowNull && value == null || value != null && Regex.IsMatch(value, _pattern))
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
