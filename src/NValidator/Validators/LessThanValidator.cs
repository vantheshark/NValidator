using System;
using System.Linq.Expressions;

namespace NValidator.Validators
{
    public sealed class LessThanValidator<T, TProperty> : ComparisonValidator<T, TProperty> where TProperty : IComparable
    {
        internal static readonly string Message = ValidatorFactory.Config.DefaultErrorMessageProvider.GetError("NValidator_Validators_LessThanValidator_Message");

        public LessThanValidator(TProperty value)
            : base(value, Message, GreaterThanOrEqualValidator<T, TProperty>.Message)
        {
        }

        public LessThanValidator(Expression<Func<T, TProperty>> expression)
            : base(expression, Message, GreaterThanOrEqualValidator<T, TProperty>.Message)
        {
        }

        public override bool IsValid(TProperty value, ValidationContext validationContext)
        {
            if (_value == null)
            {
                return value != null && ((IComparable)value).CompareTo(_value) < 0;
            }
            return ((IComparable)_value).CompareTo(value) > 0;
        }
    }
}
