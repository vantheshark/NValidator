using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NValidator.Builders;
using NValidator.Validators;

namespace NValidator
{
    public static class ValidatorExtensions
    {
        public static IPostInitFluentValidationBuilder<T, TProperty> In<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, params TProperty[] allowedValues) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new AllowedValuesValidator<TProperty>(allowedValues));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> SetValidator<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder, IValidator<TProperty> validator)
        {
            return validationBuilder.ToBuilder().SetValidator(validator);
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> Null<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder) where TProperty : class
        {
            return validationBuilder.ToBuilder().SetValidator(new IsNullValidator<TProperty>());
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> NotNull<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder) where TProperty : class
        {
            return validationBuilder.ToBuilder().SetValidator(new NotNullValidator<TProperty>());
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> NotEmpty<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder) where TProperty : IEnumerable
        {
            return validationBuilder.ToBuilder().SetValidator(new NotEmptyValidator<TProperty>());
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> Length<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, int minumumLength, int maximumLength) where TProperty : IEnumerable
        {
            return validationBuilder.ToBuilder().SetValidator(new LengthValidator<TProperty>(minumumLength, maximumLength));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> NotEqual<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder, TProperty value) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new NotEqualValidator<T, TProperty>(value));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> NotEqual<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder, Expression<Func<T, TProperty>> expression) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new NotEqualValidator<T, TProperty>(expression));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> Equal<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, TProperty value) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new EqualValidator<T, TProperty>(value));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> Equal<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, Expression<Func<T, TProperty>> expression) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new EqualValidator<T, TProperty>(expression));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> LessThan<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, TProperty value) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new LessThanValidator<T, TProperty>(value));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> LessThan<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, Expression<Func<T, TProperty>> expression) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new LessThanValidator<T, TProperty>(expression));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> LessThanOrEqual<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, TProperty value) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new LessThanOrEqualValidator<T, TProperty>(value));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> LessThanOrEqual<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, Expression<Func<T, TProperty>> expression) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new LessThanOrEqualValidator<T, TProperty>(expression));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> GreaterThan<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, TProperty value) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new GreaterThanValidator<T, TProperty>(value));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> GreaterThan<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, Expression<Func<T, TProperty>> expression) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new GreaterThanValidator<T, TProperty>(expression));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> GreaterThanOrEqual<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, TProperty value) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new GreaterThanOrEqualValidator<T, TProperty>(value));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> GreaterThanOrEqual<T, TProperty>(this IPreInitFluentValidationBuilder<T, TProperty> validationBuilder, Expression<Func<T, TProperty>> expression) where TProperty : IComparable
        {
            return validationBuilder.ToBuilder().SetValidator(new GreaterThanOrEqualValidator<T, TProperty>(expression));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> When<T, TProperty>(this IPostInitFluentValidationBuilder<T, TProperty> validationBuilder, Func<T, bool> condition)
        {
            var originalBuilder = validationBuilder.ToBuilder();
            originalBuilder.Previous.Validator = new ConditionalValidator<T, TProperty>(originalBuilder.Previous.Validator, condition);
            return validationBuilder;
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> Unless<T, TProperty>(this IPostInitFluentValidationBuilder<T, TProperty> validationBuilder, Func<T, bool> condition)
        {
            return When(validationBuilder, x => !condition(x));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> Must<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder, Func<TProperty, bool> predicate)
        {
            return validationBuilder.ToBuilder().SetValidator(new PredicateValidator<TProperty>(predicate));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> Must<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder, Func<T, TProperty, bool> predicate) where T : class
        {
            return validationBuilder.ToBuilder().SetValidator(new PredicateValidator<T, TProperty>(predicate));
        }

        public static IPostInitFluentValidationBuilder<T, string> Match<T>(this IPreInitFluentValidationBuilder<T, string> validationBuilder, string pattern)
        {
            return validationBuilder.ToBuilder().SetValidator(new RegularExpressionValidator(pattern));
        }

        public static IPostInitFluentValidationBuilder<T, string> Email<T>(this IPreInitFluentValidationBuilder<T, string> validationBuilder)
        {
            return validationBuilder.ToBuilder().SetValidator(new RegularExpressionValidator("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", "@PropertyName is not a valid email address."));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> ValidateUsing<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder, Type valiationAttributeType)
        {
            return (IPostInitFluentValidationBuilder<T, TProperty>)validationBuilder.ToBuilder().SetValidator(new DataAnnotationValidatorAdaptor(valiationAttributeType));
        }

        public static void IgnoreValidation<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder)
        {
            var originalBuilder = validationBuilder.ToBuilder();
            originalBuilder.BeforeValidation = (x, context) =>
            {
                if (context == null)
                {
                    context = new ValidationContext();
                }
                var memberName = x.GetChainName() ?? x.ContainerName;
                var ignoredList = (List<string>) context.Items[ValidationContext.IgnoredMembers];
                if (!ignoredList.Contains(memberName))
                {
                    ignoredList.Add(memberName);
                }
            };
        }

        /// <summary>
        /// Override the error message of the first validation result if the results has only 1 item. Otherwise, it returs the original result set.
        /// <para>It makes sense only if the validator produces only 1 error message such as NotNullValidator.</para>
        /// </summary>
        public static IFluentValidationBuilder<T, TProperty> WithMessage<T, TProperty>(this IPostInitFluentValidationBuilder<T, TProperty> validationBuilder, string message)
        {
            var newValidator = new EventValidator<TProperty>(validationBuilder.ToBuilder().Previous.Validator);
            newValidator.AfterValidation = x =>
            {
                if (x != null && x.Count() == 1)
                {
                    var item = x.First();
                    item.Message = message;
                    return new[] { item };
                }
                return x;
            };
            validationBuilder.ToBuilder().Previous.Validator = newValidator;
            return validationBuilder;
        }

        /// <summary>
        /// Stops validation of the chain on first error.
        /// </summary>
        public static IPostInitFluentValidationBuilder<T, TProperty> StopOnFirstError<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder)
        {
            validationBuilder.ToBuilder().StopChainOnError = true;
            return validationBuilder.ToBuilder();
        }

        public static IPreInitFluentValidationBuilder<T, TProperty> Not<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder)
        {
            var originalBuilder = validationBuilder.ToBuilder();
            originalBuilder.SetValidator(ValidatorFactory.NullValidator);
            return new NegativeValidationBuilder<T, TProperty>(originalBuilder);
        }

        public static string TryGetContainerName(this IValidator validator)
        {
            return (validator is IHaveContainer) ? (validator as IHaveContainer).ContainerName : null;
        }

        public static void TryUpdateContainerName(this IValidator validator, string containerName)
        {
            if (validator is IHaveContainer)
            {
                (validator as IHaveContainer).UpdateContainerName(containerName);
            }
        }
    }
}
