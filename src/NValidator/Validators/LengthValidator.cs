using System;
using System.Collections;
using System.Collections.Generic;

namespace NValidator.Validators
{
    public class LengthValidator<T> : BaseValidator<T> where T : IEnumerable
    {
        private readonly Func<Dictionary<string, object>> _paramResolver;
        private readonly int _minimumLength;
        private readonly int _maximumLength;
        private int _count;

        public LengthValidator(int minimumLength, int maximumLength)
        {
            _minimumLength = minimumLength;
            _maximumLength = maximumLength;
            _paramResolver = () =>  new Dictionary<string, object>
            {
                {"@MinimumLength", _minimumLength},
                {"@MaximumLength", _maximumLength},
                {"@TotalLength", _count}
            };
        }

        public override IEnumerable<ValidationResult> GetValidationResult(T value, ValidationContext validationContext)
        {
            const string defaultMessage = "@PropertyName must be between @MinimumLength and @MaximumLength in length. You had @TotalLength in length.";
            var enumerable = value as IEnumerable;
            _count = 0;
            if (enumerable != null)
            {
                foreach(var item in enumerable)
                {
                    _count++;
                }
            }
            else if (_minimumLength != 0)
            {
                yield return new FormattableMessageResult(_paramResolver())
                {
                    Message = defaultMessage
                };
                yield break;
            }

            if (_count < _minimumLength || _count > _maximumLength)
            {
                yield return new FormattableMessageResult(_paramResolver())
                {
                    Message = defaultMessage
                };
                yield break;
            }
        }
    }
}
