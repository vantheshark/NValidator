using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public static IPostInitFluentValidationBuilder<T, string> Match<T>(this IPreInitFluentValidationBuilder<T, string> validationBuilder, string pattern, bool allowNull = true)
        {
            return validationBuilder.ToBuilder().SetValidator(new RegularExpressionValidator(pattern, allowNull));
        }

        public static IPostInitFluentValidationBuilder<T, string> Email<T>(this IPreInitFluentValidationBuilder<T, string> validationBuilder)
        {
            return validationBuilder.ToBuilder().SetValidator(new RegularExpressionValidator("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", false, "@PropertyName is not a valid email address."));
        }

        public static IPostInitFluentValidationBuilder<T, TProperty> ValidateUsing<T, TProperty>(this IFluentValidationBuilder<T, TProperty> validationBuilder, Type valiationAttributeType)
        {
            if (!valiationAttributeType.Is<ValidationAttribute>())
            {
                throw new NotSupportedException("The provided Type must be a type of ValidationAttribute.");
            }
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
                var memberName = x.ChainName ?? x.ContainerName;
                var ignoredList = (List<string>)context.Items[ValidationContext.IgnoredMembers];
                if (!ignoredList.Contains(memberName))
                {
                    ignoredList.Add(memberName);
                }
            };
        }

        /// <summary>
        /// Override the error message of the first validation result if the results has only 1 item. Otherwise, it returns the original result set.
        /// <para>It makes sense only if the validator produces only 1 error message such as NotNullValidator, NotEmptyValidator, PredicateValidator, etc.</para>
        /// </summary>
        public static IFluentValidationBuilder<T, TProperty> WithMessage<T, TProperty>(this IPostInitFluentValidationBuilder<T, TProperty> validationBuilder, string message)
        {
            var newValidator = new EventValidator<TProperty>(validationBuilder.ToBuilder().Previous.Validator);
            newValidator.AfterValidation = (x, results) =>
            {
                if (results != null && results.Count() == 1)
                {
                    var item = results.First();
                    item.Message = message;
                    return new[] { item };
                }
                return results;
            };
            validationBuilder.ToBuilder().Previous.Validator = newValidator;
            return validationBuilder;
        }

        /// <summary>
        /// Override the error message of the first validation result if the results has only 1 item. Otherwise, it returns the original result set.
        /// <para>It makes sense only if the validator produces only 1 error message such as NotNullValidator, NotEmptyValidator, PredicateValidator, etc.</para>
        /// </summary>
        public static IFluentValidationBuilder<T, TProperty> WithMessage<T, TProperty>(this IPostInitFluentValidationBuilder<T, TProperty> validationBuilder, Func<T, string> message) where T : class
        {
            UpdateValidatorToReturnCustomMessage<T, TProperty>(validationBuilder.ToBuilder().Previous, message);
            return validationBuilder;
        }

        /// <summary>
        /// Override the error message of the validation result for all rules in the chain if the result set has only 1 item. Otherwise, it returns the original result set.
        /// <para>It makes sense only if the validators for current chain always produce only 1 error message such as NotNullValidator, NotEmptyValidator, PredicateValidator, etc.</para>
        /// </summary>
        public static void AllWithMessage<T, TProperty>(this IPostInitFluentValidationBuilder<T, TProperty> validationBuilder, Func<T, string> message) where T : class
        {
            var originalBuilder = validationBuilder.ToBuilder() as IValidationBuilder<T>;
            Action<IValidationBuilder<T>> updateValidators = builder => UpdateValidatorToReturnCustomMessage<T, TProperty>(builder, message);
            ValidationBuilderHelpers.UpdateBuilderChain(originalBuilder, updateValidators);
        }

        private static void UpdateValidatorToReturnCustomMessage<T, TProperty>(IValidationBuilder<T> builder, Func<T, string> message) where T : class
        {
            var newValidator = new EventValidator<T, TProperty>(builder.Validator);
            newValidator.AfterValidation = (o, vResults) =>
            {
                if (vResults != null && vResults.Count() == 1)
                {
                    var item = vResults.First();
                    item.Message = message(o);
                    return new[] { item };
                }
                return vResults;
            };
            builder.Validator = newValidator;
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

        public static void ForEach<T, TItem>(this IFluentValidationBuilder<T, TItem[]> validationBuilder, Action<TypeValidator<TItem>> rules)
        {
            var originalBuilder = validationBuilder.ToBuilder();
            var compositeBuilder = new CompositeValidationBuilder<T, TItem[], TItem>(originalBuilder);
            rules((TypeValidator<TItem>) compositeBuilder.Validator);
        }

        public static void ForEach<T, TItem>(this IFluentValidationBuilder<T, IEnumerable<TItem>> validationBuilder, Action<TypeValidator<TItem>> rules)
        {
            var originalBuilder = validationBuilder.ToBuilder();
            var compositeBuilder = new CompositeValidationBuilder<T, IEnumerable<TItem>, TItem>(originalBuilder);
            rules((TypeValidator<TItem>)compositeBuilder.Validator);
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
