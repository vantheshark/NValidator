using System;
using System.Collections.Generic;
using System.Linq;

namespace NValidator.Validators
{
    public class AllowedValuesValidator<T> : BaseNegatableValidator<T> where T : IComparable
    {
        private readonly IEnumerable<T> _allowedValues;
        private readonly bool _ignoreNull;

        public AllowedValuesValidator(IEnumerable<T> allowedValues, bool ignoreNull = false)
        {
            if (allowedValues == null)
            {
                throw new ArgumentNullException("allowedValues");
            }
            if (allowedValues.Count() == 0)
            {
                throw new ArgumentException("Allowed values must contain at least 1 item.", "allowedValues");
            }
            _allowedValues = allowedValues;
            _ignoreNull = ignoreNull;
        }

        public override IEnumerable<ValidationResult> GetNegatableErrors(T value, ValidationContext validationContext)
        {
            var values = String.Join(", ", _allowedValues);
            yield return new FormattableMessageResult(new Dictionary<string, object> { { "@ProhibitValues", values } }) { Message = ErrorMessageProvider.GetError("NValidator_Validators_AllowedValuesValidator_GetNegatableErrors") };
        }

        public sealed override bool IsValid(T value, ValidationContext validationContext)
        {
            return value == null && _ignoreNull || _allowedValues.Contains(value);
        }

        public override IEnumerable<ValidationResult> GetErrors(T value, ValidationContext validationContext)
        {
            var values = String.Join(", ", _allowedValues);
            yield return new FormattableMessageResult(new Dictionary<string, object> { { "@AllowedValues", values } }) { Message = ErrorMessageProvider.GetError("NValidator_Validators_AllowedValuesValidator_GetErrors") };
        }
    }
}
