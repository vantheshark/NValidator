using System;
using System.Collections.Generic;
using System.Linq;

namespace NValidator.Validators
{
    public class AllowedValuesValidator<T> : BaseNegatableValidator<T> where T : IComparable
    {
        private readonly IEnumerable<T> _allowedValues;

        public AllowedValuesValidator(IEnumerable<T> allowedValues)
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
        }

        public override IEnumerable<ValidationResult> GetNegatableErrors(T value, ValidationContext validationContext)
        {
            var values = String.Join(", ", _allowedValues);
            yield return new FormattableMessageResult(new Dictionary<string, object> { { "@ProhibitValues", values } }) { Message = "@PropertyName must not be one of the following values: @ProhibitValues." };
        }

        public sealed override bool IsValid(T value, ValidationContext validationContext)
        {
            return _allowedValues.Contains(value);
        }

        public override IEnumerable<ValidationResult> GetErrors(T value, ValidationContext validationContext)
        {
            var values = String.Join(", ", _allowedValues);
            yield return new FormattableMessageResult(new Dictionary<string, object> { { "@AllowedValues", values } }) { Message = "@PropertyName must be one of the allowed values. Allowed values: @AllowedValues." };
        }
    }
}
