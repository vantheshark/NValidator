using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NValidator.Validators
{
    public abstract class ComparisonValidator<T, TProperty> : IValidator<TProperty>, INegatableValidator<TProperty> where TProperty : IComparable
    {
        private readonly Expression<Func<T, TProperty>> _expression;
        private readonly string _messageFormat;
        private readonly string _negativeMessageFormat;
        protected TProperty _value;
        private readonly bool _useExpression;

        protected ComparisonValidator()
        {
        }

        protected ComparisonValidator(TProperty value, string messageFormat, string negativeMessageFormat)
        {
            _value = value;
            _messageFormat = messageFormat;
            _negativeMessageFormat = negativeMessageFormat;
        }

        protected ComparisonValidator(Expression<Func<T, TProperty>> expression, string messageFormat, string negativeMessageFormat)
        {
            _expression = expression;
            _messageFormat = messageFormat;
            _negativeMessageFormat = negativeMessageFormat;
            _useExpression = true;
        }

        public bool IsValid(TProperty value)
        {
            return IsValid(value, null);
        }

        public abstract bool IsValid(TProperty value, ValidationContext validationContext);

        public IEnumerable<ValidationResult> GetValidationResult(TProperty value)
        {
            return GetValidationResult(value, null);
        }

        public virtual IEnumerable<ValidationResult> GetValidationResult(TProperty value, ValidationContext validationContext)
        {
            if (_useExpression && validationContext.ContainerInstance is T)
            {
                _value = _expression.Compile()((T)validationContext.ContainerInstance);
            }

            if (!IsValid(value, validationContext))
            {
                return GetErrors(value, validationContext);
            }
            return Enumerable.Empty<ValidationResult>();
        }

        public virtual IEnumerable<ValidationResult> GetErrors(TProperty value, ValidationContext validationContext)
        {
            yield return new FormattableMessageResult(new Dictionary<string, object> { {"@ComparisonValue", _value}})
            {
                Message = _messageFormat
            };
        }

        public virtual IEnumerable<ValidationResult> GetNegatableErrors(TProperty value, ValidationContext validationContext)
        {
            yield return new FormattableMessageResult(new Dictionary<string, object> { { "@ComparisonValue", _value } })
            {
                Message = _negativeMessageFormat
            };
        }

        public bool IsValid(object value)
        {
            return IsValid((TProperty)value);
        }

        public bool IsValid(object value, ValidationContext validationContext)
        {
            return IsValid((TProperty) value, validationContext);
        }

        public IEnumerable<ValidationResult> GetValidationResult(object value)
        {
            return GetValidationResult((TProperty) value, null);
        }

        public IEnumerable<ValidationResult> GetValidationResult(object value, ValidationContext validationContext)
        {
            return GetValidationResult((TProperty)value, validationContext);
        }
    }
}
